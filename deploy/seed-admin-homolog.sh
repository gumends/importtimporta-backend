#!/usr/bin/env bash
set -eu

ADMIN_EMAIL="${ADMIN_EMAIL:-admin@importtimporta.com}"
ADMIN_PASSWORD="${ADMIN_PASSWORD:-Import@123}"
MSSQL_SA_PASSWORD="${MSSQL_SA_PASSWORD:-Importt@Homolog2026!}"
API_URL="${API_URL:-http://127.0.0.1:5083}"

cat > /tmp/user.json <<EOF
{"name":"Admin Homolog","email":"${ADMIN_EMAIL}","senha":"${ADMIN_PASSWORD}","nascimento":"1990-01-01"}
EOF

curl -s -X POST "${API_URL}/usuario" \
  -H "Content-Type: application/json" \
  --data-binary @/tmp/user.json
echo

sudo docker exec importt_sql_homolog /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -C -d importtimporta \
  -Q "UPDATE users SET Role = 1 WHERE Email = '${ADMIN_EMAIL}';"

echo "Admin: ${ADMIN_EMAIL} / ${ADMIN_PASSWORD}"
