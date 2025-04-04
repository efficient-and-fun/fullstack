# Stage 1: Build the React app
FROM node:18-alpine as build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build

# Stage 2: Serve the app with Nginx
FROM nginx:stable-alpine
# Remove the default Nginx configuration
RUN rm /etc/nginx/conf.d/default.conf
# Copy our custom Nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf
# Copy the React build output to the Nginx html folder
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]