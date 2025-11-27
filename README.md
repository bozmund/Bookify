# Bookify

Bookify is a book management and discovery application built with .NET. It allows users to search for books, view book details, rate and comment on books, and manage personal reading lists.

## How It Works

### Architecture

The application follows a three-tier architecture:

- **WebApi**: A REST API backend that manages book data, user ratings, comments, and reading lists. It uses Entity Framework Core with SQLite for data persistence and loads book data from a CSV file on startup.
- **Client**: A shared client library containing data models, DTOs, and a Refit-based HTTP client interface for communicating with the API.
- **BlazorWebApp**: A server-side Blazor frontend that provides the user interface using MudBlazor components. It includes user authentication via ASP.NET Identity.

### Core Features

**Book Search and Discovery**
- Users can search for books by name through the home page
- Clicking on a book navigates to a detailed book page showing cover image, title, and author information
- Related books are displayed as recommendations on the book detail page

**User Interactions** (requires authentication)
- **Rating**: Users can rate books on a 1-5 scale
- **Comments**: Users can post comments on books, which are displayed in chronological order
- **Reading Lists**: Users can mark books as "Read" or "To Read" to build personal reading lists

**Data Management**
- Books can be soft-deleted (marked as deleted without being removed from the database)
- Reading lists (Read/To Read books) and ratings are tracked per user
- Comments include timestamps and usernames

### Data Flow

1. The Blazor frontend sends HTTP requests to the WebApi using the Refit client
2. The WebApi processes requests through controller endpoints
3. Data is persisted to separate SQLite database files (one for books/comments/ratings, one for user identity)
4. Responses are returned as DTOs to maintain separation between internal models and API contracts
