
# Backend Documentation 📜

Welcome to the backend documentation for **Food Squad**. This documentation provides details about the API endpoints, entities, and their functionalities.

---

## 🌍 Live Deployment

- **Deployment**: [gm-apps](https://foodsquad.gm-apps.dev/swagger)

---

## Backend Setup 🛠️

### 1. **Clone the Repository**

   ```bash
   git clone https://github.com/Yakimov1337/FoodApp_FullStack.git
   cd FoodApp_FullStack/server
   ```

---

### 2. **Configure the Application**

   - Open the `appsettings.json` file in the `FoodSquad_API` project.
   - Update the `DefaultConnection` string with your database server details.

   Example:
   ```json
   {
       "ConnectionStrings": {
           "DefaultConnection": "Server=YourServerName;Database=FoodSquadDB;Trusted_Connection=True;MultipleActiveResultSets=true"
       }
   }
   ```

---

### 3. **Build and Run the Application**

   - Open the solution in Visual Studio 2022 or newer.
   - Restore the NuGet packages and build the project:

     ```bash
     dotnet restore
     dotnet build
     ```

   - Run the project using Visual Studio or the .NET CLI:

     ```bash
     dotnet run --project FoodSquad_API
     ```

---

### 4. **Automatic Database Setup**

   - The application is configured to automatically handle migrations and seeding. 
   - When the application is run:
     1. All pending migrations will be applied to the database.
     2. Default data (e.g., users, roles, menu items) will be seeded into the database.

   **Note**: Ensure the database server is running before starting the application.

---

### 5. **Access the API**

   - By default, the backend runs on `https://localhost:7238`.
   - Swagger documentation is available at:  
     [Swagger UI](https://localhost:7238/swagger)

   Example Screenshot of the Swagger UI:  
   ![Swagger UI Screenshot](https://i.ibb.co/DGHR5v1/Screenshot-37.png)

---

## 🚀 Overview

The **Food Squad** backend API provides endpoints for managing users, orders, reviews, and menu items. The backend includes:
- **Role-based Access Control (RBAC)** for secure endpoint access.
- A **JWT-based Authentication System**.
- A **Database Seeder** for initial data setup.
- Comprehensive **Swagger Documentation** accessible at [Swagger UI](https://localhost:7238/swagger).

---


# 🔒 Security Configuration

The application uses Spring Security to manage authentication and authorization. Below is a summary of the security configuration:

| Configuration Aspect    | Details                                                                                  |
|-------------------------|------------------------------------------------------------------------------------------|
| **CSRF Protection**     | ❌ Disabled (stateless API)                                                               |
| **Public Endpoints**    | ✔️ `/api/auth/**`, `/api/token/**`, Swagger endpoints (`/v3/api-docs/**`, `/swagger-ui/**`, `/swagger-ui.html`) |
| **Role-Based Access**   | ✔️ Admin, Moderator, Normal roles with varying permissions for Users, Orders, Menu Items, and Reviews |
| **Exception Handling**  | ✔️ Custom handlers for access denied and authentication errors                            |
| **Session Management**  | ✔️ Stateless (Session Creation Policy)                                                    |
| **JWT Filter**          | ✔️ Added before `UsernamePasswordAuthenticationFilter`                                     |
| **Authentication Manager** | ✔️ Bean provided for managing authentication                                            |

## Role-Based Access to Endpoints

| Endpoint                              | Admin | Moderator | Normal |
|---------------------------------------|--------|-----------|--------|
| **Auth Controller**                   | ✔️     | ✔️        | ✔️     |
| `/api/auth/**`                        | ✔️     | ✔️        | ✔️     |
| **Token Controller**                  | ✔️     | ✔️        | ✔️     |
| `/api/token/**`                       | ✔️     | ✔️        | ✔️     |
| **User Controller**                   | ✔️     | ✔️        | ✔️     |
| `/api/users/**`                       |        |           |        |
| - **GET**                             | ✔️     | ✔️        | ❌     |
| - **POST**                            | ✔️     | ✔️        | ❌     |
| - **PUT**                             | ✔️     | ✔️        | ✔️     |
| - **DELETE**                          | ✔️     | ❌        | ❌     |
| **Order Controller**                  | ✔️     | ✔️        | ✔️     |
| `/api/orders/**`                      |        |           |        |
| - **GET**                             | ✔️     | ✔️        | ✔️     |
| - **POST**                            | ✔️     | ✔️        | ✔️     |
| - **PUT**                             | ✔️     | ✔️        | ❌     |
| - **DELETE**                          | ✔️     | ❌        | ❌     |
| **MenuItem Controller**               | ✔️     | ✔️        | ✔️     |
| `/api/menu-items/**`                  |        |           |        |
| - **GET**                             | ✔️     | ✔️        | ✔️     |
| - **POST**                            | ✔️     | ✔️        | ❌     |
| - **PUT**                             | ✔️     | ✔️        | ❌     |
| - **DELETE**                          | ✔️     | ❌        | ❌     |
| **Reviews Controller**                | ✔️     | ✔️        | ✔️     |
| `/api/reviews/**`                     |        |           |        |
| - **GET**                             | ✔️     | ✔️        | ✔️     |
| - **POST**                            | ✔️     | ✔️        | ✔️     |
| - **PUT**                             | ✔️     | ✔️        | ❌     |
| - **DELETE**                          | ✔️     | ❌        | ❌     |

This setup ensures a secure and well-structured approach to handling API requests and protecting endpoints based on user roles.


## 🗂️ API Endpoints

### 1. User Management

#### 📋 User Entity

| Field         | Type   | Description                          |
|---------------|--------|--------------------------------------|
| `id`          | String | Unique identifier for the user       |
| `name`        | String | User's full name                     |
| `email`       | String | User's email address                 |
| `role`        | String | Role of the user (e.g., ADMIN, USER) |
| `imageUrl`    | String | URL of the user's profile image      |
| `phoneNumber` | String | User's phone number                  |

#### Endpoints

| Method | Endpoint        | Description                           | Icon      |
|--------|-----------------|---------------------------------------|-----------|
| POST   | /api/users      | Create a new user (Admin panel usage) | 📝 Create |
| GET    | /api/users      | Get all users with pagination         | 📋 List   |
| GET    | /api/users/{id} | Get a user by ID                      | 🔍 View   |
| PUT    | /api/users/{id} | Update a user by ID                   | ✏️ Update |
| DELETE | /api/users/{id} | Delete a user by ID                   | ❌ Delete  |

### 2. Menu Item Management

#### 📋 Menu Item Entity

| Field         | Type    | Description                            |
|---------------|---------|----------------------------------------|
| `id`          | Long    | Unique identifier for the item         |
| `name`        | String  | Name of the menu item                  |
| `description` | String  | Description of the item                |
| `price`       | Double  | Price of the item                      |
| `category`    | String  | Category of the item                   |
| `isDefault`   | Boolean | Whether the item is default            |
| `salesCount`  | Integer | Number of times the item has been sold |

#### Endpoints

| Method | Endpoint              | Description                              | Icon           |
|--------|-----------------------|------------------------------------------|----------------|
| POST   | /api/menu-items       | Create a new menu item                   | 📝 Create      |
| GET    | /api/menu-items/{id}  | Get a menu item by ID                    | 🔍 View        |
| GET    | /api/menu-items       | Get all menu items with optional filters | 📋 List        |
| PUT    | /api/menu-items/{id}  | Update a menu item by ID                 | ✏️ Update      |
| DELETE | /api/menu-items/{id}  | Delete a menu item by ID                 | ❌ Delete       |
| GET    | /api/menu-items/batch | Get menu items by IDs                    | 📦 Batch View  |
| DELETE | /api/menu-items/batch | Delete menu items by IDs                 | ❌ Batch Delete |

### 3. Order Management

#### 📋 Order Entity

| Field         | Type   | Description                         |
|---------------|--------|-------------------------------------|
| `id`          | String | Unique identifier for the order     |
| `userId`      | String | ID of the user who placed the order |
| `menuItems`   | List   | List of menu items in the order     |
| `totalAmount` | Double | Total amount of the order           |
| `status`      | String | Current status of the order         |
| `createdDate` | Date   | Date when the order was created     |

#### Endpoints

| Method | Endpoint                  | Description                    | Icon           |
|--------|---------------------------|--------------------------------|----------------|
| POST   | /api/orders               | Create a new order             | 📝 Create      |
| GET    | /api/orders/{id}          | Get an order by ID             | 🔍 View        |
| GET    | /api/orders/user/{userId} | Get orders by user ID          | 📋 List        |
| GET    | /api/orders               | Get all orders with pagination | 📋 List        |
| PUT    | /api/orders/{id}          | Update an order by ID          | ✏️ Update      |
| DELETE | /api/orders/{id}          | Delete an order by ID          | ❌ Delete       |
| DELETE | /api/orders/batch         | Delete orders by IDs           | ❌ Batch Delete |

### 4. Review Management

#### 📋 Review Entity

| Field         | Type    | Description                         |
|---------------|---------|-------------------------------------|
| `id`          | Long    | Unique identifier for the review    |
| `userId`      | String  | ID of the user who wrote the review |
| `menuItemId`  | Long    | ID of the menu item being reviewed  |
| `rating`      | Integer | Rating given by the user (1-5)      |
| `comment`     | String  | Review comment                      |
| `createdDate` | Date    | Date when the review was created    |

#### Endpoints

| Method   | Endpoint                            | Description                                      | Icon                  |
|----------|-------------------------------------|--------------------------------------------------|-----------------------|
| POST     | /api/reviews                        | Create a new review                              | 📝 Create             |
| GET      | /api/reviews/user/{userId}          | Get reviews by user ID                           | 📋 List               |
| GET      | /api/reviews/menu-item/{menuItemId} | Get reviews by menu item ID                      | 📋 List               |
| PUT      | /api/reviews/{id}                   | Update a review by ID                            
| Method   | Endpoint                            | Description                                      | Icon                  |
| -------- | -------------------------           | ------------------------------------------------ | --------------------- |
| PUT      | /api/reviews/{id}                   | Update a review by ID                            | ✏️ Update             |
| DELETE   | /api/reviews/{id}                   | Delete a review by ID                            | ❌ Delete              |

### 5. Authentication & Authorization

#### 📋 Token Entity

| Field          | Type   | Description                                 |
|----------------|--------|---------------------------------------------|
| `accessToken`  | String | JWT token for accessing protected resources |
| `refreshToken` | String | JWT token for refreshing access tokens      |

#### Endpoints

| Method | Endpoint                | Description                                    | Icon             |
|--------|-------------------------|------------------------------------------------|------------------|
| POST   | /api/auth/register      | Register a new user                            | 📝 Register      |
| POST   | /api/auth/login         | Authenticate a user and issue tokens           | 🔑 Login         |
| POST   | /api/auth/refresh-token | Refresh the access token using a refresh token | 🔄 Refresh Token |
| POST   | /api/auth/logout        | Logout the user and invalidate tokens          | 🚪 Logout        |

## 📜 Error Handling Documentation

This documentation provides an overview of the different error responses returned by the application and the conditions
under which they are triggered.

### Error Responses

| **Error**                                | **HTTP Status Code**        | **Description**                                           | **Condition**                                                            |
|------------------------------------------|-----------------------------|-----------------------------------------------------------|--------------------------------------------------------------------------|
| `Invalid JSON input`                     | 400 (Bad Request)           | The JSON input is malformed or invalid.                   | Triggered when a `HttpMessageNotReadableException` is thrown.            |
| `Email already exists`                   | 409 (Conflict)              | The email provided during registration is already in use. | Triggered when a `IllegalArgumentException` is thrown with this message. |
| `Authentication failed: {error message}` | 401 (Unauthorized)          | General authentication failure.                           | Triggered by `CustomAuthenticationEntryPoint` when authentication fails. |
| `Access Denied: {error message}`         | 403 (Forbidden)             | The user does not have permission to access the resource. | Triggered by `CustomAccessDeniedHandler` when access is denied.          |
| `Invalid or expired token`               | 401 (Unauthorized)          | The provided token is invalid or has expired.             | Triggered when a `JwtException` is caught.                               |
| `Token has expired`                      | 401 (Unauthorized)          | The provided token has expired.                           | Triggered when an `ExpiredJwtException` is caught.                       |
| `Invalid email or password`              | 400 (Bad Request)           | The email or password provided during login is incorrect. | Triggered when a `IllegalArgumentException` is thrown with this message. |
| `Passwords do not match`                 | 400 (Bad Request)           | The password and confirm password fields do not match.    | Triggered when a password mismatch is detected during registration.      |
| `User not found`                         | 404 (Not Found)             | The specified user could not be found.                    | Triggered when a `IllegalArgumentException` is thrown with this message. |
| `Order not found`                        | 404 (Not Found)             | The specified order could not be found.                   | Triggered when a `IllegalArgumentException` is thrown with this message. |
| `Menu Item not found`                    | 404 (Not Found)             | The specified menu item could not be found.               | Triggered when a `IllegalArgumentException` is thrown with this message. |
| `Invalid or expired refresh token`       | 403 (Forbidden)             | The provided refresh token is invalid or expired.         | Triggered when an invalid or expired refresh token is detected.          |
| `An unexpected error occurred: {error}`  | 500 (Internal Server Error) | A general, unexpected server error.                       | Triggered by a catch-all `Exception` handler.                            |
| `Field validation errors`                | 400 (Bad Request)           | One or more fields failed validation.                     | Triggered when a `MethodArgumentNotValidException` is thrown.            |

### Exception Handling

**1. Invalid JSON Input**

- **Exception**: `HttpMessageNotReadableException`
- **Response Code**: 400
- **Message**: "Invalid JSON input"

**2. Email Already Exists**

- **Exception**: `IllegalArgumentException` (with message: "Email already exists")
- **Response Code**: 409
- **Message**: "Email already exists"

**3. Authentication Failed**

- **Exception**: `CustomAuthenticationEntryPoint`
- **Response Code**: 401
- **Message**: "Authentication failed: {error message}"

**4. Access Denied**

- **Exception**: `CustomAccessDeniedHandler`
- **Response Code**: 403
- **Message**: "Access Denied: {error message}"

**5. Invalid or Expired Token**

- **Exception**: `JwtException`
- **Response Code**: 401
- **Message**: "Invalid or expired token"

**6. Token Expired**

- **Exception**: `ExpiredJwtException`
- **Response Code**: 401
- **Message**: "Token has expired"

**7. Invalid Email or Password**

- **Exception**: `IllegalArgumentException` (with message: "Invalid email or password")
- **Response Code**: 400
- **Message**: "Invalid email or password"

**8. Passwords Do Not Match**

- **Exception**: Validation check during registration
- **Response Code**: 400
- **Message**: "Passwords do not match"

**9. User Not Found**

- **Exception**: `IllegalArgumentException` (with message: "User not found")
- **Response Code**: 404
- **Message**: "User not found"

**10. Order Not Found**

- **Exception**: `IllegalArgumentException` (with message: "Order not found")
- **Response Code**: 404
- **Message**: "Order not found"

**11. Menu Item Not Found**

- **Exception**: `IllegalArgumentException` (with message: "Menu Item not found")
- **Response Code**: 404
- **Message**: "Menu Item not found"

**12. Invalid or Expired Refresh Token**

- **Exception**: Validation check during refresh token processing
- **Response Code**: 403
- **Message**: "Invalid or expired refresh token"

**13. Unexpected Error**

- **Exception**: Catch-all `Exception` handler
- **Response Code**: 500
- **Message**: "An unexpected error occurred: {error}"

**14. Field Validation Errors**

- **Exception**: `MethodArgumentNotValidException`
- **Response Code**: 400
- **Message**: "Field validation errors"

## 🌟 Development & Testing

For local development and testing, ensure that your environment variables are properly configured and that you have set
up your database schema according to the provided SQL scripts. Use the included Postman collection to test the API
endpoints.

## 📝 Contributing

If you have any suggestions or find issues, please feel free to contribute by submitting pull requests or reporting
issues on our GitHub repository.

Thank you for using Food Squad's backend API!
