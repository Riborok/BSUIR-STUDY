import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
from sklearn.decomposition import PCA
from sklearn.cluster import KMeans
from sklearn.preprocessing import StandardScaler

# Set style
sns.set(style="whitegrid")

def load_and_clean_data(filepath):
    df = pd.read_excel(filepath)

    # 1. Clean 'Like' column
    # Replace 'I love it!+5' with 5 and 'I hate it!-5' with -5
    df['Like'] = df['Like'].replace({'I love it!+5': 5, 'I hate it!-5': -5})
    df['Like'] = pd.to_numeric(df['Like'])

    # 2. Convert Yes/No to 1/0 for perception attributes
    perception_cols = ['yummy', 'convenient', 'spicy', 'fattening', 'greasy',
                       'fast', 'cheap', 'tasty', 'expensive', 'healthy', 'disgusting']

    for col in perception_cols:
        df[col] = df[col].map({'Yes': 1, 'No': 0})

    # Map Visit Frequency
    # Correct mapping based on actual unique values in the dataset
    visit_freq_map = {
        'Never': 0,
        'Once a year': 1,
        'Every three months': 4,
        'Once a month': 12,
        'Once a week': 52,
        'More than once a week': 104
    }
    df['VisitFrequencyNumeric'] = df['VisitFrequency'].map(visit_freq_map)

    # Check for unmapped values
    if df['VisitFrequencyNumeric'].isnull().any():
        print("Warning: Some VisitFrequency values were not mapped and are NaN:")
        print(df[df['VisitFrequencyNumeric'].isnull()]['VisitFrequency'].unique())
        # Fill NaNs with median or 0? Let's drop them or fill with 0 for now to avoid errors,
        # but better to know. For now, let's assume the map covers all.
        # If not, they will be excluded from plots.

    # Map Gender
    df['GenderNumeric'] = df['Gender'].map({'Female': 0, 'Male': 1})

    return df, perception_cols

def analyze_outliers(df, column):
    Q1 = df[column].quantile(0.25)
    Q3 = df[column].quantile(0.75)
    IQR = Q3 - Q1
    lower_bound = Q1 - 1.5 * IQR
    upper_bound = Q3 + 1.5 * IQR

    outliers = df[(df[column] < lower_bound) | (df[column] > upper_bound)]

    # Plot visualization
    plt.figure(figsize=(10, 6))

    # Check if column is binary (only 0 and 1, or just two values)
    unique_vals = df[column].unique()
    is_binary = len(unique_vals) <= 2 and all(x in [0, 1] for x in unique_vals if not np.isnan(x))

    if is_binary:
        # Use Countplot for binary features
        sns.countplot(x=column, data=df, palette='viridis')
        plt.title(f'Распределение значений: {column} (Бинарный признак)')
        plt.ylabel('Количество')
    else:
        # Use Boxplot for continuous/ordinal features
        # Make outliers red and larger to be clearly visible
        sns.boxplot(x=df[column], color='lightgreen',
                    flierprops={"marker": "o", "markerfacecolor": "red", "markeredgecolor": "red", "markersize": 6})
        plt.title(f'Анализ выбросов: {column}')

        # If the column is VisitFrequencyNumeric, add a swarmplot to show distribution better
        if column == 'VisitFrequencyNumeric':
             sns.stripplot(x=df[column], color='black', alpha=0.3, jitter=True)

    plt.xlabel(column)
    plt.savefig(f'{column.lower()}_outliers.png')
    plt.close()

    return len(outliers), lower_bound, upper_bound

def plot_univariate(df):
    # Age Distribution
    plt.figure(figsize=(10, 6))
    sns.histplot(df['Age'], bins=20, kde=True, color='skyblue')
    plt.title('Распределение возраста респондентов')
    plt.xlabel('Возраст')
    plt.ylabel('Количество')
    plt.savefig('age_distribution_new.png')
    plt.close()

    # Like Distribution
    plt.figure(figsize=(10, 6))
    sns.countplot(x='Like', data=df, palette='viridis')
    plt.title('Распределение оценки "Like"')
    plt.xlabel('Оценка (от -5 до +5)')
    plt.ylabel('Количество')
    plt.savefig('like_distribution_new.png')
    plt.close()

def plot_bivariate(df):
    # Like vs Age
    plt.figure(figsize=(10, 6))
    sns.boxplot(x='Like', y='Age', data=df, palette='coolwarm')
    plt.title('Взаимосвязь оценки "Like" и возраста')
    plt.xlabel('Оценка')
    plt.ylabel('Возраст')
    plt.savefig('like_vs_age.png')
    plt.close()

def perform_pca(df, cols):
    # PCA
    pca = PCA(n_components=2)
    principalComponents = pca.fit_transform(df[cols])
    pca_df = pd.DataFrame(data=principalComponents, columns=['PC1', 'PC2'])

    # Plot PCA
    plt.figure(figsize=(12, 10))
    sns.scatterplot(x='PC1', y='PC2', data=pca_df, alpha=0.3, color='lightgray')

    # Plot loadings (attributes)
    loadings = pca.components_.T * np.sqrt(pca.explained_variance_)

    # Scale factor for arrows to make them visible
    scale_arrow = 1.0

    for i, feature in enumerate(cols):
        plt.arrow(0, 0, loadings[i, 0] * scale_arrow, loadings[i, 1] * scale_arrow,
                  color='r', alpha=0.8, head_width=0.05, head_length=0.1)

        # Add offset to text to avoid overlapping
        x_offset = loadings[i, 0] * scale_arrow * 1.15
        y_offset = loadings[i, 1] * scale_arrow * 1.15

        # Adjust alignment based on quadrant
        ha = 'center'
        va = 'center'
        if x_offset > 0: ha = 'left'
        if x_offset < 0: ha = 'right'
        if y_offset > 0: va = 'bottom'
        if y_offset < 0: va = 'top'

        plt.text(x_offset, y_offset, feature, color='darkblue',
                 ha=ha, va=va, fontsize=12, fontweight='bold')

    plt.title('Карта восприятия (PCA)')
    plt.xlabel(f'PC1 ({pca.explained_variance_ratio_[0]:.2%} variance)')
    plt.ylabel(f'PC2 ({pca.explained_variance_ratio_[1]:.2%} variance)')
    plt.grid(True)
    plt.savefig('pca_perception_map_new.png')
    plt.close()

    return pca

def perform_clustering(df, cols):
    # Elbow Method
    wcss = []
    for i in range(1, 11):
        kmeans = KMeans(n_clusters=i, init='k-means++', random_state=42)
        kmeans.fit(df[cols])
        wcss.append(kmeans.inertia_)

    plt.figure(figsize=(10, 6))
    plt.plot(range(1, 11), wcss, marker='o')
    plt.title('Метод локтя для определения оптимального k')
    plt.xlabel('Количество кластеров')
    plt.ylabel('WCSS')
    plt.savefig('elbow_method_new.png')
    plt.close()

    # Cluster Profiling (let's choose k=4 based on typical results for this dataset)
    k = 4
    kmeans = KMeans(n_clusters=k, init='k-means++', random_state=42)
    df['Cluster'] = kmeans.fit_predict(df[cols])

    # Profile plot
    cluster_means = df.groupby('Cluster')[cols].mean()

    plt.figure(figsize=(14, 8))
    sns.heatmap(cluster_means.T, cmap='YlGnBu', annot=True, fmt='.2f')
    plt.title('Профиль кластеров (средние значения атрибутов)')
    plt.savefig('cluster_profile_new.png')
    plt.close()

def main():
    print("Loading data...")
    df, perception_cols = load_and_clean_data('mcdonalds.xlsm')

    print(f"Data shape: {df.shape}")
    print(f"Duplicates: {df.duplicated().sum()}")

    # Outliers analysis for all numeric columns
    # User requested analysis for ALL columns including binary ones
    all_numeric_cols = ['Age', 'Like', 'VisitFrequencyNumeric', 'GenderNumeric'] + perception_cols

    print("Analyzing outliers for all columns...")
    for col in all_numeric_cols:
        n_outliers, lb, ub = analyze_outliers(df, col)
        print(f"Outliers in {col}: {n_outliers} (Bounds: {lb:.2f}, {ub:.2f})")

    print("Generating univariate plots...")
    plot_univariate(df)

    print("Generating bivariate plots...")
    plot_bivariate(df)

    print("Performing PCA...")
    perform_pca(df, perception_cols)

    print("Performing Clustering...")
    perform_clustering(df, perception_cols)

    print("Analysis complete.")

if __name__ == "__main__":
    main()
