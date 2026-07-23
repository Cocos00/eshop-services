import pandas as pd

# Leer CSV exportado
df = pd.read_csv("data/sensorData.csv")

# Mantener solo la columna Distance
df = df[["Distance"]]

# Convertir a número
df["Distance"] = pd.to_numeric(df["Distance"], errors="coerce")

# Eliminar valores nulos
df = df.dropna()

# Eliminar distancias inválidas
df = df[(df["Distance"] >= 2) & (df["Distance"] <= 400)]

# Eliminar duplicados
df = df.drop_duplicates()

# Crear categoría

def classify(distance):

    if distance <= 10:
        return "Muy Cerca"

    elif distance <= 30:
        return "Cerca"

    elif distance <= 70:
        return "Media"

    elif distance <= 150:
        return "Lejos"

    else:
        return "Muy Lejos"

df["Category"] = df["Distance"].apply(classify)

df.to_csv("data/sensorData_ML.csv", index=False)

print(df.head())

print()

print("Dataset preparado correctamente")