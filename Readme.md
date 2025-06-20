# Púrpura Music Backend

<p align="center">
  <img alt="VS Code in action" src="https://res.cloudinary.com/dw43hgf5p/image/upload/v1734400648/p1j2larqesr3qi4yn3o5.png" width="300">
</p>

Backend for the Púrpura Music streaming platform, built with ASP.NET Core and PostgreSQL.  
This API powers the music streaming, user management, playlists, recommendations, and more.

## 🌐 Public Deployment

- **Frontend:** [https://purpura-music.vercel.app/](https://purpura-music.vercel.app/)


---

## 🚀 Features

- **User Authentication & Authorization**
  - JWT-based authentication
  - Registration, login, password recovery, email verification
  - Role-based access (admin/user)

- **Music Streaming**
  - Stream songs directly from the API
  - Search for songs, albums, and artists

- **User Library**
  - Save songs, albums, and playlists to your personal library
  - View and manage your saved content

- **Playlists**
  - Create, update, and delete playlists
  - Add or remove songs from playlists
  - Public and private playlists

- **Albums & Artists**
  - Browse albums and artists
  - View top songs and albums

- **Recommendations**
  - Personalized daily playlist ("Purple Day List") based on listening history
  - Genre-based top songs

- **Admin Features**
  - Manage songs, albums, artists, genres (CRUD)
  - Only accessible to users with the `ADMIN` role

- **Email Notifications**
  - Account verification, password recovery, and password change notifications

---

## 📚 API Endpoints (Examples)

- `POST /api/auth/register` – Register a new user
- `POST /api/auth/login` – User login and token generation
- `GET /api/users/{id}` – Get user profile
- `GET /api/songs` – List all songs
- `GET /api/songs/stream/{id}` – Stream a song
- `GET /api/playlists` – Get user playlists
- `POST /api/playlists` – Create a playlist
- `GET /api/search?query={text}` – Search for songs, albums, or artists


---

## 🛠️ Technologies Used

- **Backend:** ASP.NET Core 9.0
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Authentication:** JWT, Identity Framework
- **Frontend:** Next.js ([see deployment](https://purpura-music.vercel.app/))
- **Cloud Storage:** Cloudinary (for images/audio)
- **Email:** Gmail SMTP via MailKit

---

## 🏗️ Project Structure

- `Controllers/` – API endpoints
- `Services/` – Business logic (Implementations & Interfaces)
- `Dto/` – Data transfer objects
- `Model/` – Entity models
- `DbContext/` – EF Core context
- `Migrations/` – Database migrations
- `Utils/`, `Validations/`, `Exceptions/` – Helpers and error handling

---

## 🚀 Getting Started (Development)

1. **Clone the repository**
    ```sh
    git clone https://github.com/yourusername/purpuraMusicBack.git
    cd purpuraMusicBack/purpuraMain
    ```

2. **Configure environment**
    - Set up your `appsettings.json` and `appsettings.Development.json` with your DB, JWT, Cloudinary, and Gmail credentials.

3. **Run migrations**
    ```sh
    dotnet ef database update
    ```

4. **Run the app**
    ```sh
    dotnet run
    ```

5. **Docker (optional)**
    ```sh
    docker build -t purpura-music-backend .
    docker run -p 8080:80 purpura-music-backend
    ```

---

## 📄 License

This project is licensed under the MIT License.

---

## 👤 About the Creator

Created by **Cristian David Vargas Loaiza**  
[LinkedIn](https://www.linkedin.com/in/cristian-david-vargas-loaiza-982314271) | [GitHub](https://github.com/CrisD314159) | [Portfolio](https://crisdev-pi.vercel.app)
