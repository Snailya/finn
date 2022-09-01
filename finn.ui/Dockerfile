FROM node:alpine

WORKDIR /app

COPY package*.json ./
RUN npm config set registry http://registry.npm.taobao.org
RUN npm install react-scripts -g silent serve
RUN npm ci

COPY . ./

CMD ["npm", "run", "start"]