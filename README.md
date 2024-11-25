
# 🍔 Food Squad 🍽️

Welcome to **Food Squad**, an advanced web application designed to enhance your dining experience. **Food Squad** integrates Angular 17 on the frontend with Java Spring Boot 3 for backend services, offering a robust and scalable solution for food enthusiasts. For more detailed information, please check client/server README file.

## 🌍 Live Deployment

- **Deployment**: [Vercel](https://food-app-full-stack.vercel.app/) 

## 🚀 Key Features

- **🍏 Fully Responsive Design**: Optimized for seamless use on all devices.
- **🔍 Theme Customization**: Switch between light and dark themes with 7 unique color schemes.
- **💳 Secure Payments**: Integrated with Stripe for reliable and secure transactions. (Use 424242... for test transactions)
- **📄 Profile Management**: Allows users to update profiles and upload images for a personalized experience.

## 🛠️ Tech Stack

- **🔹 Frontend**: Angular 17, Tailwind CSS for styling.
- **🔸 Backend**: Java Spring Boot 3 for a robust server-side experience.
- **🌐 Payment Integration**: Stripe for secure financial transactions.

## 📸 Pages Overview

### 🔍 Admin Dashboard
![Admin Dashboard](https://i.ibb.co/XSz8YRr/Screenshot-2024-04-03-232830.png)

**Designed for administrators to oversee operations and manage content:**
- **Overview**: Insights into revenue and performance.
- **CRUD Operations**: Manage menu items, orders, and users.

### 🏠 Home Page
![Home Page](https://i.ibb.co/fHgVrP7/Screenshot-2024-04-03-232914.png)

**The welcoming interface showcasing latest offers and features.**

### 🛒 Menu Page
![Menu Page](https://i.ibb.co/8d4QwpY/Screenshot-2024-04-03-232927.png)

**Displays a categorized list of available food items.**

### ☘️ Categories Page
![Categories Page](https://i.ibb.co/KNDx6MY/Screenshot-2024-04-03-232943.png)

**Browse menu items by category for easier navigation.**

### 👩‍🎓 Role-Based Page
![Role-Based Page](https://i.ibb.co/CJ3KR6q/Screenshot-2024-04-03-235001.png)

**Customizes the interface based on user roles for a tailored experience.**

### 🙅‍♂️ Sign Up Page
![Sign Up Page](https://i.ibb.co/X3F01MD/Screenshot-2024-04-03-234925.png)

**Simple and secure registration process for new users.**

### 🤵️ Account Settings
![Account Settings](https://i.ibb.co/ykwWdY6/Screenshot-2024-04-03-235418.png)

**Manage personal details, preferences, and security settings.**

### 💰 Pricing Plans
![Pricing Plans](https://i.ibb.co/kH1zvw4/Screenshot-2024-04-03-232953.png)

**Explore various subscription options tailored to user needs.**

## 👤 Permissions Overview

| Feature                | Admin | Moderator |
|------------------------|:-----:|:---------:|
| Delete Menu Items      |  ✔️   |     ❌    |
| Update Default Items   |  ✔️   |     ❌    |
| Update Non-default Items |  ✔️   |    ✔️    |
| CRUD Orders            |  ✔️   |     Limited |
| CRUD Users             |  ✔️   | Limited   |
| CRUD Reviews           |  ✔️   |     Limited |

## 🏠 Routing Logic

| Route                         | Unauthenticated | Authenticated (Normal) | Authenticated (Admin/Moderator) |
|-------------------------------|:---------------:|:-----------------------:|:--------------------------------:|
| /auth/sign-up, /auth/sign-in  |        ✔️       |            ❌           |                 ❌               |
| Home, Contact, About, etc.    |        ❌       |            ✔️           |                 ✔️               |
| Admin Dashboard               |        ❌       |            ❌           |                 ✔️               |

## 📦 Installation and Setup

### Frontend Setup

1. **Clone the Repository**
    ```bash
    git clone https://github.com/Yakimov1337/FoodApp_FullStack/tree/main/client
    cd fullstack-food-squad/client
    ```

2. **Install Dependencies**
    ```bash
    npm install
    ```

3. **Update `environment.local.ts`** with your API endpoint and Stripe key.

4. **Start the Frontend**
    ```bash
    ng serve
    ```

### Backend Setup

1. **Navigate to the Backend Directory**
    ```bash
    cd ../server
    ```

2. **Build and Run the Backend Server**
    - Ensure you have Java 17 and Maven installed.
    ```bash
    ./mvnw clean package
    ./mvnw spring-boot:run
    ```

3. **Configure the Backend**
    - Update `src/main/resources/application.properties` with your database and environment configurations.

4. **Access the Backend**
    - By default, the backend runs on `http://localhost:8080`.

## 🖥️ Usage

This project is intended for educational purposes. It utilizes Hero Icons and Hero Patterns for visual elements.

---
Feel free to customize the above documentation according to any specific details or additional features relevant to your project!
