#!/bin/bash
set -e

echo "Inicializando ZooTech MongoDB..."

mongosh \
  --username "$MONGODB_INITDB_ROOT_USERNAME" \
  --password "$MONGODB_INITDB_ROOT_PASSWORD" \
  --authenticationDatabase admin <<EOF

// ============================================================
// ZooTech - Base de datos de auditoría
// ============================================================

db = db.getSiblingDB("$MONGODB_DATABASE_NAME");

// ============================================================
// Usuario utilizado por ZooTech.Infrastructure
// ============================================================

db.createUser({
    user: "$MONGODB_APP_USER",
    pwd: "$MONGODB_APP_PASSWORD",
    roles: [
        {
            role: "readWrite",
            db: "$MONGODB_DATABASE_NAME"
        }
    ]
});

// ============================================================
// Colección de auditoría de eventos
// ============================================================

db.createCollection("$MONGODB_EVENT_LOGS_COLLECTION_NAME");

// ============================================================
// Colección de auditoría de errores
// ============================================================

db.createCollection("$MONGODB_ERROR_LOGS_COLLECTION_NAME");

EOF

echo "MongoDB inicializado correctamente."