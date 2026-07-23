import joblib

model = joblib.load("models/decision_tree.pkl")

distance = float(input("Ingrese distancia: "))

prediction = model.predict([[distance]])

print()

print("Predicción")

print(prediction[0])