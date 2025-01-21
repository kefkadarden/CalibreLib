# CalibreLib

CalibreLib is a C# web application designed to interact with and manage Calibre, an e-book library manager. This application provides tools to automate tasks such as adding, removing, and organizing e-books in a Calibre database through a web interface.

## Features

- Web-based interface to manage Calibre library
- Add and remove e-books from the Calibre library
- Search and organize e-books
- Export and import Calibre database

## Installation

### Prerequisites

- .NET SDK (version 8.0 or later)
- Visual Studio 2022

### Steps

1. Clone the repository:

```bash
git clone https://github.com/kefkadarden/CalibreLib.git
cd CalibreLib
```

2. Restore the dependencies:

```bash
dotnet restore
```

3. Build the application:

```bash
dotnet build
```

4. Run the application:

```bash
dotnet run
```

The application should now be running on `http://localhost:7119`.

## Contributing

Contributions are welcome! Please follow these steps to contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature`).
3. Commit your changes (`git commit -am 'Add some feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Create a new Pull Request.

## License

This project is licensed under the GPL3 License - see the [LICENSE](LICENSE) file for details.

## Acknowledgements

- [Calibre](https://calibre-ebook.com/) - The e-book management software that this application interacts with.