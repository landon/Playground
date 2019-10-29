import dash
import dash_core_components as dcc
import dash_html_components as html
import pandas as pd
import numpy as np
import plotly.graph_objs as go
import pywt
import itertools as it


def transform(data):
    return list(it.chain(*pywt.wavedec(data, 'db1')))

def transform_clip_inverse(data):
    coeffs = pywt.wavedec(data, 'db1')
    for i in range(1, len(coeffs)):
        coeffs[i] = np.array([c if abs(c) >= 10 else 0 for c in coeffs[i]])
    return pywt.waverec(coeffs, 'db1')

app = dash.Dash(__name__)

df = pd.read_csv('https://raw.githubusercontent.com/plotly/datasets/master/2016-weather-data-seattle.csv')
df = df.dropna()
df["Date"] = pd.to_datetime(df["Date"])
df["year"] = df["Date"].dt.year

df = df[df["year"] < 1960]

xx = np.linspace(0, len(df), len(df))

app.layout = html.Div([
    dcc.Graph(
        id='test',
        figure={
            'data': [
                go.Scatter(x=xx, y=df["Max_TemperatureC"], name="ddd", mode='lines',)
            ],
            'layout': go.Layout()
        }
    ),
    dcc.Graph(
        id='test2',
        figure={
            'data': [
                go.Scatter(x=xx, y=transform(df["Max_TemperatureC"]), name="ddd", mode='lines',)
            ],
            'layout': go.Layout()
        }
    ),
    dcc.Graph(
        id='test3',
        figure={
            'data': [
                go.Scatter(x=xx, y=transform_clip_inverse(df["Max_TemperatureC"]), name="ddd", mode='lines',)
            ],
            'layout': go.Layout()
        }
    )
])



if __name__ == '__main__':
    app.run_server(debug=True)