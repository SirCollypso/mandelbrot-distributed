# mandelbrot-distributed
## _A Mandelbrot set fractal multicore distributed generator_

Mandelbrot Distributed is a multicore distributed Mandelbrot set fractal generator. Broker connects a distributor, result saver, and workers. This makes it possible to distribute generation and storage of the fractal image. 

## Disclaimer
This project was created by a thirteen-year-old me as a multicore threading and socket programming exercise.

## Features

- Distributes calculations among several devices
- Utilizes multicores for parallel calculations
- Saves the image on any desired device

## Usage

Run [Broker](https://github.com/SirCollypso/mandelbrot-distributed/tree/main/Broker)
*Broker* accepts two arguments:
```sh
ip-address [default = 127.0.0.1]
port [default = 13000]
```
Run [ResultStore](https://github.com/SirCollypso/mandelbrot-distributed/blob/main/ResultStore)
*ResultStore* accepts two arguments:
```sh
ip-address [default = 127.0.0.1]
port [default = 10000]
```
Run [Distributor](https://github.com/SirCollypso/mandelbrot-distributed/blob/main/Distributor)
*Distributor* accepts four arguments:
```sh
broker-ip-address [default = 127.0.0.1]
broker-port [default = 10000]
jobID [default = 1]
pixels [default = 500]
```
Run [Worker](https://github.com/SirCollypso/mandelbrot-distributed/tree/main/Worker)
*Worker* accepts four arguments:
```sh
broker-ip-address [default = 127.0.0.1]
broker-port [default = 10000]
result-saver-ip-address [default = 127.0.0.1]
result-saver-port [default = 13000]
```


