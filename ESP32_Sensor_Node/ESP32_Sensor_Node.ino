#include <WiFi.h>
#include <HTTPClient.h>

// ==========================================
// CONFIGURACIÓN DE RED (Cambia estos valores)
// ==========================================
const char* ssid = "TU_WIFI_SSID";
const char* password = "TU_WIFI_PASSWORD";

// ==========================================
// CONFIGURACIÓN DE API (Cambia la IP por la de tu PC)
// ==========================================
// Nota: La IP debe ser la IP local de la computadora donde corre el Docker/API (Ej: 192.168.1.45)
// El puerto 5000 es el expuesto en docker-compose.override.yml para catalog.api
const char* serverName = "http://192.168.1.XXX:5000/api/sensor"; 

// ==========================================
// PINES DEL SENSOR ULTRASÓNICO (HC-SR04)
// ==========================================
const int trigPin = 5;  // Pin D5 en la ESP32
const int echoPin = 18; // Pin D18 en la ESP32

// Variables para cálculos
long duration;
double distanceCm;

void setup() {
  Serial.begin(115200);

  // Configuración de pines del sensor
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);

  // Iniciar conexión WiFi
  WiFi.begin(ssid, password);
  Serial.println("Conectando a WiFi...");
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  
  Serial.println("\nConectado a la red WiFi exitosamente.");
  Serial.print("Dirección IP asignada: ");
  Serial.println(WiFi.localIP());
}

void loop() {
  // Comprobar estado de WiFi
  if(WiFi.status() == WL_CONNECTED){
    HTTPClient http;

    // --- 1. LEER EL SENSOR ULTRASÓNICO ---
    digitalWrite(trigPin, LOW);
    delayMicroseconds(2);
    // Generar pulso de 10 microsegundos
    digitalWrite(trigPin, HIGH);
    delayMicroseconds(10);
    digitalWrite(trigPin, LOW);

    // Leer el tiempo de viaje del eco
    duration = pulseIn(echoPin, HIGH);
    // Calcular distancia (velocidad del sonido: 343m/s -> 0.034 cm/us)
    distanceCm = duration * 0.034 / 2;

    Serial.print("Distancia medida: ");
    Serial.print(distanceCm);
    Serial.println(" cm");

    // --- 2. ENVIAR DATOS A LA API ---
    http.begin(serverName);
    http.addHeader("Content-Type", "application/json");

    // Crear payload JSON simple
    String jsonPayload = "{\"distance\": " + String(distanceCm) + "}";
    
    // Realizar POST
    int httpResponseCode = http.POST(jsonPayload);
    
    if (httpResponseCode > 0) {
      Serial.print("Código de respuesta HTTP: ");
      Serial.println(httpResponseCode);
      String response = http.getString();
      Serial.println("Respuesta del servidor: " + response);
    } else {
      Serial.print("Error al realizar la petición POST. Código de error: ");
      Serial.println(httpResponseCode);
    }
    
    // Liberar recursos de conexión
    http.end();
  } else {
    Serial.println("Desconectado de WiFi. Intentando reconectar...");
    // Podrías implementar lógica de reconexión si es necesario
  }

  // Esperar 15 segundos antes de la siguiente medición
  delay(15000);
}
