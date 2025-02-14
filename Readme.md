## API Endpoints  

### Authentication  
- `POST /api/auth/register` – Register a new user  
- `POST /api/auth/login` – User login and token generation  

### User Management  
- `GET /api/users/{id}` – Get user profile  
- `PUT /api/users/{id}` – Update user profile  
- `DELETE /api/users/{id}` – Delete user account  

### Music and Playback  
- `GET /api/songs` – Get all available songs  
- `GET /api/songs/{id}` – Get details of a specific song  
- `GET /api/songs/stream/{id}` – Stream a song  

### Playlists  
- `GET /api/playlists` – Get user playlists  
- `POST /api/playlists` – Create a new playlist  
- `PUT /api/playlists/{id}` – Update playlist details  
- `DELETE /api/playlists/{id}` – Delete a playlist  

### Search  
- `GET /api/search?query={text}` – Search for songs, albums, or artists  

## Technologies Used  
- **Backend**: ASP.NET Core, Node.js  
- **Database**: PostgreSQL (or your preferred DB)  
- **Authentication**: JWT  
- **Frontend Integration**: Next.js (for the frontend of Púrpura Music)  

## Contributing  
Feel free to submit issues or pull requests. Contributions are welcome!  

## License  
This project is licensed under the MIT License.  