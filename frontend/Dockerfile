FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
# Use template so nginx picks up BACKEND_URL env var via envsubst
COPY nginx.conf.template /etc/nginx/templates/default.conf.template
EXPOSE 8080
