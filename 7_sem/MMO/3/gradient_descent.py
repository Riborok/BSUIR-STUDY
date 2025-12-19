import numpy as np
import pandas as pd
from sklearn.preprocessing import StandardScaler
import matplotlib.pyplot as plt

def mserror(X, w, y):
    y_pred = X.dot(w)
    return np.mean((y_pred - y) ** 2)

def gradient_descent(X, y, eta, max_iter, eps):
    X = np.array(X, dtype=float)
    y = np.array(y, dtype=float)
    n_samples, n_features = X.shape
    w = np.zeros(n_features)
    errors = []
    num_iter = 0
    for k in range(max_iter):
        num_iter += 1
        y_pred = X.dot(w)
        grad = 2.0 / n_samples * X.T.dot(y_pred - y)
        new_w = w - eta * grad

        if np.linalg.norm(new_w - w) < eps:
            w = new_w
            break

        w = new_w
        errors.append(mserror(X, w, y))

    print(f"Число итераций: {num_iter}")
    return w, np.array(errors)

def stochastic_gradient_descent(X, y, eta, max_iter, eps):
    X = np.array(X, dtype=float)
    y = np.array(y, dtype=float)
    n_samples, n_features = X.shape
    w = np.zeros(n_features)
    errors = []
    num_iter = 0
    for k in range(max_iter):
        num_iter += 1
        i = np.random.randint(0, n_samples)
        y_pred_i = X[i].dot(w)
        grad = 2.0 * X[i] * (y_pred_i - y[i])
        new_w = w - eta * grad

        if np.linalg.norm(new_w - w) < eps:
            w = new_w
            break
        w = new_w
        errors.append(mserror(X, w, y))
    print(f"Число итераций: {num_iter}")
    return w, np.array(errors)


df = pd.read_csv("CarPrice_Assignment.csv")

df = df.drop(columns=["car_ID", "CarName"])

df["doornumber"] = df["doornumber"].map({"two": 2, "four": 4})
df["cylindernumber"] = df["cylindernumber"].map({
    "two": 2, "three": 3, "four": 4, "five": 5, "six": 6, "eight": 8, "twelve": 12
})

X = df.drop(columns=["price"])
y = df["price"]

X = pd.get_dummies(X, drop_first=True)

scaler = StandardScaler()
X_scaled = scaler.fit_transform(X)

w, errors = gradient_descent(X_scaled, y, eta=0.05, max_iter=500000, eps=1e-8)
w_sgd, errors_sgd = stochastic_gradient_descent(X_scaled, y, eta=0.0001, max_iter=500000, eps=1e-8)

y_pred = X_scaled.dot(w)

plt.figure(figsize=(8,5))
plt.plot(errors, label="Batch GD")
plt.plot(errors_sgd, label="Stochastic GD")
plt.xlabel("Итерации")
plt.ylabel("MSE")
plt.title("Сравнение сходимости: Batch vs Stochastic GD")
plt.legend()
plt.yscale("log")
plt.grid(True)
plt.show()
