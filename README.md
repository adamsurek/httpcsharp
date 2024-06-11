## HTTP/1.1 Implementation in C#

_Based on [RFC 2616](https://datatracker.ietf.org/doc/html/rfc2616)_

## Project Purpose

The purpose of this project is to gain a better understanding of the inner workings of HTTP 
and as an introduction to designing and working with network protocols. This is **not** a 
feature complete implementation of RFC 2616.

## Features

- HTTP request parsing
- Asynchronous request responses
  - `OPTION` and `GET` methods are currently available. Support for other methods are in progess
  - Allows for the transfer of text and images
- Basic file-based routing
  - `Resources/site/SitePermissions` XML defines available files and request methods
  - `Resources` can be fetched


