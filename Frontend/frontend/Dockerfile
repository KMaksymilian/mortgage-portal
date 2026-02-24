# 1. Budowanie aplikacji React
FROM node:20 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# 2. Serwowanie przez Nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html
# Usuwamy domyślny plik nginx
RUN rm -rf ./*
# Kopiujemy zbudowanego Reacta
COPY --from=build /app/dist .
# Konfiguracja, żeby React Router działał przy odświeżaniu strony
RUN echo 'server { \
    listen 80; \
    location / { \
        root /usr/share/nginx/html; \
        index index.html index.htm; \
        try_files $uri $uri/ /index.html; \
    } \
    location /api/ { \
        proxy_pass http://app:8080; \
        proxy_http_version 1.1; \
        proxy_set_header Upgrade $http_upgrade; \
        proxy_set_header Connection keep-alive; \
        proxy_set_header Host $host; \
        proxy_cache_bypass $http_upgrade; \
    } \
}' > /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]