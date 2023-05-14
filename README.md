# mandelbrot-distributed
## _A distributed Mandelbrot set fractal generator_

Mandelbrot Distributed is a fractal generator for the Mandelbrot set that operates in a distributed manner. The Broker serves as a link between the distributor, result saver, and workers, allowing for the distribution of both the generation and storage of the fractal image.

## Features

- Divides computations among multiple devices
- Harnesses the power of multiple cores for concurrent processing
- Enables users to store the generated image on a device of their choice

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


