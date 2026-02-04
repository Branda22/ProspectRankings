#!/usr/bin/env bash
set -euo pipefail

# Load .env if present
if [ -f .env ]; then
  set -a
  source .env
  set +a
fi

# ─── Configuration ───────────────────────────────────────────────
PROJECT_ID="${GCP_PROJECT_ID:?Set GCP_PROJECT_ID}"
REGION="${GCP_REGION:-us-central1}"
DB_INSTANCE="rankle-db"
DB_NAME="prospectdb"
DB_USER="admin"
DB_PASSWORD="${DB_PASSWORD:?Set DB_PASSWORD}"
JWT_SECRET="${JWT_SECRET:?Set JWT_SECRET}"
REPO="rankle"
BACKEND_IMAGE="${REGION}-docker.pkg.dev/${PROJECT_ID}/${REPO}/backend"
FRONTEND_IMAGE="${REGION}-docker.pkg.dev/${PROJECT_ID}/${REPO}/frontend"

echo "==> Project: ${PROJECT_ID}, Region: ${REGION}"

# ─── 1. Enable required APIs ────────────────────────────────────
echo "==> Enabling GCP APIs..."
gcloud services enable \
  run.googleapis.com \
  sqladmin.googleapis.com \
  artifactregistry.googleapis.com \
  cloudbuild.googleapis.com \
  --project="${PROJECT_ID}"

# ─── 2. Create Artifact Registry repo ───────────────────────────
echo "==> Creating Artifact Registry repository..."
gcloud artifacts repositories describe "${REPO}" \
  --location="${REGION}" --project="${PROJECT_ID}" 2>/dev/null || \
gcloud artifacts repositories create "${REPO}" \
  --repository-format=docker \
  --location="${REGION}" \
  --project="${PROJECT_ID}"

# Configure Docker auth
gcloud auth configure-docker "${REGION}-docker.pkg.dev" --quiet

# ─── 3. Create Cloud SQL instance ───────────────────────────────
echo "==> Creating Cloud SQL PostgreSQL instance..."
if ! gcloud sql instances describe "${DB_INSTANCE}" --project="${PROJECT_ID}" 2>/dev/null; then
  gcloud sql instances create "${DB_INSTANCE}" \
    --database-version=POSTGRES_15 \
    --tier=db-f1-micro \
    --region="${REGION}" \
    --project="${PROJECT_ID}" \
    --storage-size=10GB \
    --storage-auto-increase

  gcloud sql users set-password "${DB_USER}" \
    --instance="${DB_INSTANCE}" \
    --password="${DB_PASSWORD}" \
    --project="${PROJECT_ID}"

  gcloud sql databases create "${DB_NAME}" \
    --instance="${DB_INSTANCE}" \
    --project="${PROJECT_ID}"
fi

# Get the Cloud SQL connection name
CONNECTION_NAME=$(gcloud sql instances describe "${DB_INSTANCE}" \
  --project="${PROJECT_ID}" --format="value(connectionName)")
echo "==> Cloud SQL connection: ${CONNECTION_NAME}"

# ─── 4. Build and push backend ──────────────────────────────────
echo "==> Building backend image..."
docker build --platform linux/amd64 -t "${BACKEND_IMAGE}" ./backend
docker push "${BACKEND_IMAGE}"

# ─── 5. Deploy backend to Cloud Run ─────────────────────────────
echo "==> Deploying backend to Cloud Run..."
gcloud run deploy rankle-api \
  --image="${BACKEND_IMAGE}" \
  --platform=managed \
  --region="${REGION}" \
  --project="${PROJECT_ID}" \
  --allow-unauthenticated \
  --port=8080 \
  --set-env-vars="ASPNETCORE_ENVIRONMENT=Production" \
  --set-env-vars="JwtSettings__Secret=${JWT_SECRET}" \
  --set-env-vars="ConnectionStrings__DefaultConnection=Host=/cloudsql/${CONNECTION_NAME};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}" \
  --add-cloudsql-instances="${CONNECTION_NAME}" \
  --min-instances=0 \
  --max-instances=3 \
  --memory=512Mi

# Get the backend URL
BACKEND_URL=$(gcloud run services describe rankle-api \
  --region="${REGION}" --project="${PROJECT_ID}" \
  --format="value(status.url)")
echo "==> Backend URL: ${BACKEND_URL}"

# ─── 6. Build and push frontend ─────────────────────────────────
echo "==> Building frontend image..."
docker build --platform linux/amd64 -t "${FRONTEND_IMAGE}" ./frontend
docker push "${FRONTEND_IMAGE}"

# ─── 7. Deploy frontend to Cloud Run ────────────────────────────
echo "==> Deploying frontend to Cloud Run..."
gcloud run deploy rankle-web \
  --image="${FRONTEND_IMAGE}" \
  --platform=managed \
  --region="${REGION}" \
  --project="${PROJECT_ID}" \
  --allow-unauthenticated \
  --port=8080 \
  --set-env-vars="BACKEND_URL=${BACKEND_URL}" \
  --min-instances=0 \
  --max-instances=3 \
  --memory=256Mi

# Get the frontend URL
FRONTEND_URL=$(gcloud run services describe rankle-web \
  --region="${REGION}" --project="${PROJECT_ID}" \
  --format="value(status.url)")

echo ""
echo "==> Deployment complete!"
echo "    Frontend: ${FRONTEND_URL}"
echo "    Backend:  ${BACKEND_URL}"
