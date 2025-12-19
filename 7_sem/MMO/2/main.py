import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import missingno as msno
import os
from IPython.display import display
from sklearn.impute import SimpleImputer
from sklearn.preprocessing import LabelEncoder, OneHotEncoder, StandardScaler, MinMaxScaler

DATA_PATH = "CarPrice_Assignment.csv"


def load_data():
    pd.set_option('display.max_columns', None)
    return pd.read_csv(DATA_PATH)


def encode_categorical_features(df):
    if 'price' in df.columns and 'CarName' in df.columns:
        return encode_car_categorical(df)
    elif 'Animal ID' in df.columns and 'DateTime' in df.columns:
        return encode_animal_categorical(df)
    return df


def encode_car_categorical(df):
    print("Кодирование категориальных признаков для автомобильного датасета...")

    binary_features = ['fueltype', 'aspiration', 'doornumber', 'enginelocation']
    ohe_features = ['carbody', 'drivewheel', 'enginetype', 'fuelsystem']

    for feature in binary_features:
        if feature in df.columns:
            le = LabelEncoder()
            df[f'{feature}_encoded'] = le.fit_transform(df[feature].astype(str))
            print(f"Бинарное кодирование: {feature}")

    for feature in ohe_features:
        if feature in df.columns:
            ohe = OneHotEncoder(sparse_output=False, drop='first')
            encoded = ohe.fit_transform(df[[feature]])
            feature_names = [f"{feature}_{cat}" for cat in ohe.categories_[0][1:]]
            encoded_df = pd.DataFrame(encoded, columns=feature_names, index=df.index)
            df = pd.concat([df, encoded_df], axis=1)
            print(f"One-Hot кодирование: {feature}")

    cylinder_mapping = {'two': 2, 'three': 3, 'four': 4, 'five': 5, 'six': 6, 'eight': 8, 'twelve': 12}
    if 'cylindernumber' in df.columns:
        df['cylindernumber_numeric'] = df['cylindernumber'].map(cylinder_mapping)
        print("Порядковое кодирование: cylindernumber")

    if 'CarName' in df.columns:
        df['brand_extracted'] = df['CarName'].str.split().str[0].str.lower()
        brand_counts = df['brand_extracted'].value_counts()
        rare_brands = brand_counts[brand_counts < 3].index
        df['brand_grouped'] = df['brand_extracted'].replace(rare_brands, 'other')

        ohe = OneHotEncoder(sparse_output=False, drop='first')
        brand_encoded = ohe.fit_transform(df[['brand_grouped']])
        brand_names = [f"brand_{cat}" for cat in ohe.categories_[0][1:]]
        brand_df = pd.DataFrame(brand_encoded, columns=brand_names, index=df.index)
        df = pd.concat([df, brand_df], axis=1)
        print("One-Hot кодирование: brand (с группировкой редких)")

    return df


def encode_animal_categorical(df):
    print("Кодирование категориальных признаков для датасета с животными...")

    binary_features = ['Animal Type']
    ohe_features = ['Intake Type', 'Intake Condition', 'Sex upon Intake']

    for feature in binary_features:
        if feature in df.columns:
            le = LabelEncoder()
            df[f'{feature}_encoded'] = le.fit_transform(df[feature].astype(str))
            print(f"Бинарное кодирование: {feature}")

    for feature in ohe_features:
        if feature in df.columns:
            ohe = OneHotEncoder(sparse_output=False, drop='first')
            encoded = ohe.fit_transform(df[[feature]])
            feature_names = [f"{feature.replace(' ', '_')}_{cat}" for cat in ohe.categories_[0][1:]]
            encoded_df = pd.DataFrame(encoded, columns=feature_names, index=df.index)
            df = pd.concat([df, encoded_df], axis=1)
            print(f"One-Hot кодирование: {feature}")

    if 'Breed' in df.columns:
        breed_counts = df['Breed'].value_counts()
        common_breeds = breed_counts[breed_counts >= 10].index
        df['breed_common'] = df['Breed'].apply(lambda x: x if x in common_breeds else 'Other')

        ohe = OneHotEncoder(sparse_output=False, drop='first')
        breed_encoded = ohe.fit_transform(df[['breed_common']])
        breed_names = [f"breed_{cat.replace(' ', '_').replace('/', '_')}" for cat in ohe.categories_[0][1:]]
        breed_df = pd.DataFrame(breed_encoded, columns=breed_names, index=df.index)
        df = pd.concat([df, breed_df], axis=1)
        print("One-Hot кодирование: Breed (с группировкой редких)")

    return df


def standardize_features(df):
    print("Стандартизация числовых признаков...")

    numeric_cols = df.select_dtypes(include=[np.number]).columns.tolist()

    if len(numeric_cols) == 0:
        print("Нет числовых столбцов для стандартизации!")
        return df

    df_scaled = df.copy()

    scaler = StandardScaler()
    scaled_data = scaler.fit_transform(df[numeric_cols])

    for i, col in enumerate(numeric_cols):
        df_scaled[f'{col}_standardized'] = scaled_data[:, i]

    return df_scaled


def normalize_features(df):
    print("Нормализация числовых признаков...")

    numeric_cols = df.select_dtypes(include=[np.number]).columns.tolist()

    if len(numeric_cols) == 0:
        print("Нет числовых столбцов для стандартизации!")
        return df

    df_normalized = df.copy()

    scaler = MinMaxScaler()
    normalized_data = scaler.fit_transform(df[numeric_cols])

    for i, col in enumerate(numeric_cols):
        df_normalized[f'{col}_normalized'] = normalized_data[:, i]

    return df_normalized


def basic_info(df):
    print("=" * 100)
    print("Shape:", df.shape)
    print("=" * 100 + "\n\n")

    print("=" * 100)
    display(df.head(3).iloc[:, :5])
    print("=" * 100 + "\n\n")

    print("=" * 100)
    display(df.tail(3).iloc[:, :5])
    print("=" * 100 + "\n\n")

    print("=" * 100)
    df.info()
    print("=" * 100 + "\n\n")

    print("=" * 100)
    display(df.describe(include='all').T)
    print("=" * 100 + "\n\n")


def show_missing_analysis(df):
    msno.matrix(df)
    plt.show()

    msno.heatmap(df)
    plt.show()


def impute_missing_values(df):
    numeric_cols = df.select_dtypes(include=[np.number]).columns
    cat_cols = df.select_dtypes(include="object").columns

    if len(numeric_cols) > 0:
        num_imputer = SimpleImputer(strategy="median")
        df[numeric_cols] = num_imputer.fit_transform(df[numeric_cols])
        print(f"Заполнены пропущенные значения в числовых столбцах: {list(numeric_cols)}")

    if len(cat_cols) > 0:
        cat_imputer = SimpleImputer(strategy="most_frequent")
        df[cat_cols] = cat_imputer.fit_transform(df[cat_cols])
        print(f"Заполнены пропущенные значения в категориальных столбцах: {list(cat_cols)}")

    return df


def generate_features(df):
    if 'price' in df.columns and 'CarName' in df.columns:
        return generate_car_features(df)
    elif 'Animal ID' in df.columns and 'DateTime' in df.columns:
        return generate_animal_features(df)
    else:
        print("Неизвестный тип датасета. Невозможно сгенерировать специфические признаки.")
        return df


def generate_car_features(df):
    print("Генерируем новые признаки для автомобильного датасета...")

    if all(col in df.columns for col in ['carlength', 'carwidth', 'carheight']):
        df['size_index'] = df['carlength'] * df['carwidth'] * df['carheight']
        print("Добавлен признак 'size_index' (индекс габаритов)")

    if 'citympg' in df.columns and 'highwaympg' in df.columns:
        df['avg_mpg'] = (df['citympg'] + df['highwaympg']) / 2
        print("Добавлен признак 'avg_mpg' (средний расход топлива)")

    return df


def generate_animal_features(df):
    print("Генерируем новые признаки для датасета с животными...")

    if 'DateTime' in df.columns:
        df['DateTime'] = pd.to_datetime(df['DateTime'])

        def get_season(month):
            if month in [12, 1, 2]:
                return 'Winter'
            elif month in [3, 4, 5]:
                return 'Spring'
            elif month in [6, 7, 8]:
                return 'Summer'
            else:
                return 'Fall'

        df['season'] = df['DateTime'].dt.month.apply(get_season)
        print("Добавлен признак 'season' (сезон поступления)")

    if 'Age upon Intake' in df.columns:
        def categorize_age(age_str):
            if pd.isna(age_str):
                return 'Unknown'
            age_str = str(age_str).lower()
            if 'year' in age_str:
                try:
                    years = int(age_str.split()[0])
                    if years < 1:
                        return 'Young'
                    elif years < 5:
                        return 'Adult'
                    else:
                        return 'Senior'
                except:
                    return 'Unknown'
            elif 'month' in age_str or 'week' in age_str or 'day' in age_str:
                return 'Young'
            else:
                return 'Unknown'

        df['age_group'] = df['Age upon Intake'].apply(categorize_age)
        print("Добавлен признак 'age_group' (возрастная группа)")

    return df


def save_processed_data(df):
    dir_name, base_name = os.path.split(DATA_PATH)
    new_file_name = "PROCESSED_" + base_name
    new_path = os.path.join(dir_name, new_file_name)
    df.to_csv(new_path, index=False)
    print(f"Данные сохранены в файл: {new_path}")


def show_numeric_plots(df):
    numeric_cols = df.select_dtypes(include=[np.number]).columns

    if len(numeric_cols) > 0:
        df[numeric_cols].hist(bins=20, figsize=(15, 10))
        plt.show()

        plt.figure(figsize=(15, 8))
        df[numeric_cols].boxplot(rot=90)
        plt.title("Boxplot для числовых признаков")
        plt.show()
    else:
        print("В этом датасете нет числовых столбцов для анализа.")


def remove_outliers(df):
    numeric_cols = df.select_dtypes(include=[np.number]).columns

    if len(numeric_cols) > 0:
        def remove_outliers_iqr_all(data, numeric_columns):
            df_clean = data.copy()
            for col in numeric_columns:
                Q1 = df_clean[col].quantile(0.25)
                Q3 = df_clean[col].quantile(0.75)
                IQR = Q3 - Q1
                lower = Q1 - 1.5 * IQR
                upper = Q3 + 1.5 * IQR
                df_clean = df_clean[(df_clean[col] >= lower) & (df_clean[col] <= upper)]
            return df_clean

        df_clean = remove_outliers_iqr_all(df, numeric_cols)

        print("До:", df.shape)
        print("После удаления выбросов:", df_clean.shape)

        return df_clean
    else:
        print("В этом датасете нет числовых столбцов для анализа.")
        return df


def show_menu():
    print("\n" + "~" * 60)
    print("МЕНЮ АНАЛИЗА ДАННЫХ")
    print("~" * 60)
    print("0  - Заново прочитать исходные данные")
    print("1  - Показать базовую информацию о данных")
    print("2  - Показать анализ пропущенных значений")
    print("3  - Заполнить пропущенные значения")
    print("4  - Показать графики числовых признаков")
    print("5  - Удалить выбросы")
    print("6  - Сгенерировать новые признаки")
    print("7  - Кодировать категориальные признаки")
    print("8  - Стандартизировать признаки")
    print("9  - Нормализировать признаки")
    print("10 - Сохранить текущий датасет")
    print("11 - Выход")
    print("~" * 60)


def main():
    df = load_data()
    print("Данные загружены успешно!")

    while True:
        show_menu()
        try:
            choice = input("Выберите опцию (0-11): ").strip()

            if choice == '0':
                df = load_data()
                print("Данные перезагружены из исходного файла!")

            elif choice == '1':
                basic_info(df)

            elif choice == '2':
                show_missing_analysis(df)

            elif choice == '3':
                df = impute_missing_values(df)
                print("Пропущенные значения заполнены!")

            elif choice == '4':
                show_numeric_plots(df)

            elif choice == '5':
                df = remove_outliers(df)

            elif choice == '6':
                original_columns = len(df.columns)
                df = generate_features(df)
                new_columns = len(df.columns)
                print(f"Добавлено {new_columns - original_columns} новых признаков!")
                print(f"Общее количество признаков: {new_columns}")

            elif choice == '7':
                original_columns = len(df.columns)
                df = encode_categorical_features(df)
                new_columns = len(df.columns)
                print(f"Добавлено {new_columns - original_columns} закодированных признаков!")
                print(f"Общее количество признаков: {new_columns}")

            elif choice == '8':
                df = standardize_features(df)

            elif choice == '9':
                df = normalize_features(df)

            elif choice == '10':
                save_processed_data(df)

            elif choice == '11':
                print("Выход из программы.")
                break

            else:
                print("Неверный выбор! Пожалуйста, выберите число от 0 до 13.")

        except Exception as e:
            print(f"Произошла ошибка: {e}")


if __name__ == "__main__":
    main()