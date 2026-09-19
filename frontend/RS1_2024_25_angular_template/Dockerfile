# Build Angular app
FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json* ./
RUN npm install

COPY . ./
RUN npm run build -- --configuration production

# Serve with nginx
FROM nginx:1.27-alpine AS final
COPY nginx.docker.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/RS1_2024_25_angular_template/browser /usr/share/nginx/html

EXPOSE 80

HEALTHCHECK --interval=10s --timeout=5s --start-period=10s --retries=5 \
  CMD wget -qO- http://127.0.0.1/ >/dev/null || exit 1
