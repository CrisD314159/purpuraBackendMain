# Purpura Music Backend

This is the backend for the Purpura Music application, built with ASP.NET Core.

## Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/purpuraMusicBack.git
    ```
2. Navigate to the project directory:
    ```sh
    cd purpuraMusicBack
    ```
3. Restore the dependencies:
    ```sh
    dotnet restore
    ```
4. Build the project:
    ```sh
    dotnet build
    ```

## Usage

1. Update the [appsettings.json](http://_vscodecontentref_/1) and [appsettings.Development.json](http://_vscodecontentref_/2) files with your configuration.
2. Run the project:
    ```sh
    dotnet run
    ```

## Project Structure


## API Endpoints

### User

- **Send Recovery Mail**
    ```http
    POST /user/sendRecoveryMail?email={email}
    ```
- **Get User by ID**
    ```http
    GET /user/{id}
    ```
- **Get User by Email**
    ```http
    GET /user/getByEmail/{email}
    ```
- **Create User**
    ```http
    POST /user
    ```
- **Update User**
    ```http
    PUT /user
    ```

## Rest of the services will be added soon!!!
