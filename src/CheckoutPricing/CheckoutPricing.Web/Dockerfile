# Use an official Node.js runtime as a parent image
FROM node:14

# Set the working directory in the container
WORKDIR /app

# Copy package.json and package-lock.json to the working directory
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code to the working directory
COPY CheckoutPricing.Web/. .

# Build the React app for production
RUN npm run build

# Install a simple HTTP server to serve the static files
RUN npm install -g serve

# Set the command to run the app
CMD ["serve", "-s", "build"]

# Expose port 3000 to the outside world
EXPOSE 3000
