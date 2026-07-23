import pandas as pd

from sklearn.model_selection import train_test_split

from sklearn.tree import DecisionTreeClassifier

from sklearn.metrics import accuracy_score

import joblib

df = pd.read_csv("data/sensorData_ML.csv")

X = df[["Distance"]]

y = df["Category"]

X_train, X_test, y_train, y_test = train_test_split(
    X,
    y,
    test_size=0.20,
    random_state=42
)

model = DecisionTreeClassifier(random_state=42)

model.fit(X_train, y_train)

prediction = model.predict(X_test)

accuracy = accuracy_score(y_test, prediction)

print(f"Accuracy: {accuracy*100:.2f}%")

joblib.dump(model, "models/decision_tree.pkl")

print("Modelo guardado")