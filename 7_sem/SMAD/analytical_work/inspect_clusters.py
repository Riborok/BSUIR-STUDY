import pandas as pd
from sklearn.cluster import KMeans

def load_and_clean_data(filepath):
    df = pd.read_excel(filepath)
    # Convert Yes/No to 1/0 for perception attributes
    perception_cols = ['yummy', 'convenient', 'spicy', 'fattening', 'greasy',
                       'fast', 'cheap', 'tasty', 'expensive', 'healthy', 'disgusting']
    for col in perception_cols:
        df[col] = df[col].map({'Yes': 1, 'No': 0})
    return df, perception_cols

def inspect_clusters():
    df, cols = load_and_clean_data('mcdonalds.xlsm')

    # Add Age and Like to analysis
    df['Like'] = df['Like'].replace({'I love it!+5': 5, 'I hate it!-5': -5})
    df['Like'] = pd.to_numeric(df['Like'])

    k = 4
    kmeans = KMeans(n_clusters=k, init='k-means++', random_state=42)
    df['Cluster'] = kmeans.fit_predict(df[cols])

    # Calculate means for perception cols AND Age, Like
    analysis_cols = cols + ['Age', 'Like']
    cluster_means = df.groupby('Cluster')[analysis_cols].mean()

    with open('cluster_stats.txt', 'w') as f:
        f.write(str(cluster_means.T))
        f.write("\n\nCluster sizes:\n")
        f.write(str(df['Cluster'].value_counts().sort_index()))

    print(cluster_means.T)

if __name__ == "__main__":
    inspect_clusters()
