import numpy as np
import pandas as pd
from sklearn.preprocessing import StandardScaler
import matplotlib.pyplot as plt


def mserror(X, w, y):
    y_pred = X.dot(w)
    return np.mean((y_pred - y) ** 2)

def gradient_descent_L1(X, y, eta, max_iter, eps, lambda_):
    X = np.array(X, dtype=float)
    y = np.array(y, dtype=float)
    n_samples, n_features = X.shape
    w = np.zeros(n_features)
    errors = []
    num_iter = 0
    for k in range(max_iter):
        num_iter += 1
        y_pred = X.dot(w)
        grad = (2.0 / n_samples) * X.T.dot(y_pred - y) + lambda_ * np.sign(w)
        new_w = w - eta * grad

        if np.linalg.norm(new_w - w) < eps:
            w = new_w
            break

        w = new_w
        errors.append(mserror(X, w, y))

    print(f"Итераций (L1): {num_iter}, Lambda: {lambda_}")
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


lambdas = np.logspace(-3, 3, 20)
weights = []

for lambda_ in lambdas:
    w_l1, _ = gradient_descent_L1(X_scaled, y, eta=0.05, max_iter=50_000, eps=1e-6, lambda_=lambda_)
    weights.append(w_l1)

weights = np.array(weights)

plt.figure(figsize=(10,6))
for i in range(weights.shape[1]):
    plt.plot(lambdas, weights[:, i], label=f'w{i}', alpha=0.6)

plt.xscale('log')
plt.xlabel("λ (коэффициент регуляризации)")
plt.ylabel("Значение весов")
plt.title("Изменение весов при L1-регуляризации")
plt.grid(True)
plt.show()
