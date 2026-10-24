#!/usr/bin/env bash
set -euo pipefail

PUBLIC_URL="${PUBLIC_URL:-http://52.91.185.36}"
API_PUBLIC_URL="${API_PUBLIC_URL:-http://52.91.185.36:5083}"
MSSQL_SA_PASSWORD="${MSSQL_SA_PASSWORD:-Importt@Homolog2026!}"
JWT_KEY="${JWT_KEY:-importtimporta-homolog-jwt-key-32chars!!}"
ADMIN_EMAIL="${ADMIN_EMAIL:-admin@importtimporta.com}"
ADMIN_PASSWORD="${ADMIN_PASSWORD:-Import@123}"

export PUBLIC_URL API_PUBLIC_URL MSSQL_SA_PASSWORD JWT_KEY

sudo apt-get update -y
sudo apt-get install -y git nginx

if ! command -v docker >/dev/null 2>&1; then
  curl -fsSL https://get.docker.com | sudo sh
  sudo usermod -aG docker "$USER" || true
fi

sudo mkdir -p /opt/importtimporta
sudo chown -R "$USER:$USER" /opt/importtimporta

if [ ! -d /opt/importtimporta/importtimporta-backend/.git ]; then
  git clone https://github.com/gumends/importtimporta-backend.git /opt/importtimporta/importtimporta-backend
fi

if [ ! -d /opt/importtimporta/importtimporta/.git ]; then
  git clone https://github.com/gumends/importtimporta.git /opt/importtimporta/importtimporta
fi

cd /opt/importtimporta/importtimporta-backend
git pull origin main

cd /opt/importtimporta/importtimporta
git pull origin main

cd /opt/importtimporta/importtimporta-backend
docker compose -f docker-compose.homolog.yml up -d --build

echo "Aguardando API..."
for i in $(seq 1 60); do
  if curl -sf "$API_PUBLIC_URL/swagger/index.html" >/dev/null 2>&1; then
    break
  fi
  sleep 5
done

curl -sf -X POST "$API_PUBLIC_URL/usuario" \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Admin Homolog\",\"email\":\"$ADMIN_EMAIL\",\"senha\":\"$ADMIN_PASSWORD\",\"nascimento\":\"1990-01-01\"}" \
  >/dev/null 2>&1 || true

docker exec importt_sql_homolog /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q \
  "UPDATE users SET Role = 1 WHERE Email = '$ADMIN_EMAIL';" >/dev/null 2>&1 || true

cd /opt/importtimporta/importtimporta
docker build --build-arg NEXT_PUBLIC_API_URL="$API_PUBLIC_URL" -t importtimporta-frontend .
docker rm -f importt_front_homolog 2>/dev/null || true
docker run -d --name importt_front_homolog --restart unless-stopped -p 3000:3000 importtimporta-frontend

sudo tee /etc/nginx/sites-available/importtimporta >/dev/null <<NGINX
server {
    listen 80 default_server;
    listen [::]:80 default_server;
    server_name _;

    location / {
        proxy_pass http://127.0.0.1:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
    }
}
NGINX

sudo ln -sf /etc/nginx/sites-available/importtimporta /etc/nginx/sites-enabled/importtimporta
sudo rm -f /etc/nginx/sites-enabled/default
sudo nginx -t
sudo systemctl reload nginx

echo ""
echo "=== Homolog pronta ==="
echo "Site:    $PUBLIC_URL"
echo "API:     $API_PUBLIC_URL"
echo "Admin:   $ADMIN_EMAIL / $ADMIN_PASSWORD"
echo "Swagger: $API_PUBLIC_URL/swagger"
