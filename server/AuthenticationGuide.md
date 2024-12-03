
# README: FoodSquad Authentication System

## **Overview**

The FoodSquad authentication system uses **JWT (JSON Web Tokens)** for secure and efficient user authentication and session management. The system incorporates both **Access Tokens** (short-lived) and **Refresh Tokens** (long-lived) to minimize re-authentication while maintaining security. This document explains the components, their purpose, and how they work together.

---

## **Why Two Tokens?**
1. **Access Token**:
   - Short-lived (~30 minutes).
   - Used for every API request to authenticate the user.
   - Reduces server-side validation overhead by carrying all required claims.
   - Expiry ensures that any token compromise is short-lived.

2. **Refresh Token**:
   - Long-lived (~7 days).
   - Stored in cookies (HTTP-only) for enhanced security.
   - Used to generate new Access Tokens when the existing one expires.
   - Server-side validation ensures compromised tokens can be invalidated immediately.

---

## **How Authentication Works**

### **1. Login Workflow**
   - **Step 1**: User submits credentials (`email` and `password`) to the `/api/auth/sign-in` endpoint.
   - **Step 2**: The server validates credentials using `_authService.LoginUserAsync`.
   - **Step 3**: 
     - Generates an **Access Token** with user claims like `email`, `role`, `id`, `name`, etc.
     - Generates a **Refresh Token** with similar claims and a longer expiration.
   - **Step 4**: Tokens are stored:
     - **Access Token**: Sent back to the client for use in API requests.
     - **Refresh Token**: Stored in an HTTP-only cookie for enhanced security.

   ```json
   {
       "accessToken": "eyJhbGciOiJIUzI1NiIsInR..."
   }
   ```

---

### **2. Using the Access Token**
   - The client app includes the `accessToken` in the `Authorization` header for every API request:
     ```
     Authorization: Bearer <accessToken>
     ```
   - The `JwtRequestFilter` middleware extracts and validates the token. Claims (`email`, `role`, etc.) are mapped to `HttpContext.User` for authorization.

---

### **3. Refresh Token Workflow**
   - When the Access Token expires, the client sends a request to `/api/token/refresh-token`.
   - The server validates the **Refresh Token** from cookies:
     - Extracts user claims (e.g., `email`, `role`).
     - Ensures the token exists in the database (via `_tokenService.IsRefreshTokenValidAsync`).
   - If valid:
     - Generates a new pair of Access and Refresh Tokens.
     - Invalidates the old Refresh Token in the database.
     - Updates the cookie with the new Refresh Token.

---

### **4. Token Expiry and Re-Authentication**
   - If the Refresh Token is expired or invalid, the user must log in again.
   - This ensures that long-term access cannot be abused without explicit re-authentication.

---

## **Core Components**

### **1. JwtUtil**
   - Handles token generation and validation.
   - Provides `GenerateToken()` to create tokens with claims like `email`, `role`, `id`, etc.
   - Includes `ExtractClaims()` to decode and verify token claims.
   - Configures `TokenValidationParameters` for the JWT middleware.

   Example:
   ```csharp
   public string GenerateToken(
       string email,
       UserRole role,
       Guid id,
       string name,
       string phoneNumber,
       string imageUrl,
       long expiration)
   ```

### **2. JwtRequestFilter**
   - A middleware that maps JWT claims to `HttpContext.User` for easier role-based access control.
   - Explicitly maps `email` and `role` claims to standard `ClaimTypes`.

   ```csharp
   public async Task InvokeAsync(HttpContext context, RequestDelegate next)
   ```

---

### **3. SecurityConfig**
   - Registers the JWT authentication scheme with token validation parameters.
   - Adds event handlers for:
     - **OnTokenValidated**: Ensures claims like `role` and `email` are mapped.
     - **OnAuthenticationFailed**: Handles expired/invalid Access Tokens.

   ```csharp
   services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.Events = new JwtBearerEvents
           {
               OnAuthenticationFailed = context =>
               {
                   // Returns an "Access token expired" response for expired tokens.
               }
           };
       });
   ```

---

### **4. TokenController**
   - **`POST /refresh-token`**:
     - Validates the Refresh Token from cookies.
     - Generates new Access and Refresh Tokens.
     - Returns the new Access Token in the response.

   - **`POST /sign-in`**:
     - Authenticates the user and returns Access and Refresh Tokens.

   - **`GET /current-user`**:
     - Returns the currently authenticated user's details.

---

### **5. Custom Middleware**
   - **`CustomAccessDeniedHandler`**: Handles unauthorized requests by returning detailed JSON responses.
   - Example:
     ```json
     {
         "error": "Unauthorized",
         "message": "Access token is missing or expired."
     }
     ```

---

### **6. Frontend Integration**
   - **Interceptor Workflow**:
     - The Angular frontend uses an `HttpInterceptor` to include the `accessToken` in API requests.
     - On `401 Unauthorized`:
       - Sends a request to `/refresh-token`.
       - Updates the `accessToken` and retries the failed request.
       - If both tokens are invalid, redirects the user to the login page.

   ```typescript
   intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
       return next.handle(request).pipe(
           catchError((error: HttpErrorResponse) => {
               if (error.status === 401) {
                   // Refresh token logic
               }
           })
       );
   }
   ```

---

## **Examples**

### **Sign-In Request**
   ```http
   POST /api/auth/sign-in
   {
       "email": "admin@example.com",
       "password": "securepassword"
   }
   ```

   **Response**:
   ```json
   {
       "accessToken": "eyJhbGciOiJIUzI1NiIsInR5c..."
   }
   ```

---

### **Refresh Token Request**
   ```http
   POST /api/token/refresh-token
   Cookie: refreshToken=<your-refresh-token>
   ```

   **Response**:
   ```json
   {
       "accessToken": "newAccessToken"
   }
   ```

---

### **Error Responses**
   - **Access Token Expired**:
     ```json
     {
         "error": "Unauthorized",
         "message": "Access token is missing or expired."
     }
     ```

   - **Refresh Token Invalid**:
     ```json
     {
         "error": "Unauthorized",
         "message": "Refresh token is invalid or expired."
     }
     ```

---

## **Why This Design?**
- **Two Tokens**: Separating short-lived Access Tokens from long-lived Refresh Tokens improves security and usability.
- **Centralized Validation**: By storing Refresh Tokens in the database, you can revoke tokens at any time, controlling user sessions.
- **Middleware Logic**: Ensures clean separation of authentication and authorization logic.
- **Frontend Handling**: Automatic retry logic enhances user experience without manual intervention.

---

## **Conclusion**
This authentication system is designed for **security**, **scalability**, and **user convenience**. By combining token-based authentication with robust middleware and frontend retry logic, it ensures a seamless and secure user experience.
