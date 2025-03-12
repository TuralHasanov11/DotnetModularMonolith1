#!/bin/bash

# Apply Namespace
kubectl apply -f k8s/modularmonolith-namespace.yaml

# Apply Secrets and ConfigMaps
kubectl apply -f k8s/postgres-secret.yaml
kubectl apply -f k8s/pgadmin-secret.yaml
kubectl apply -f k8s/postgres-configmap.yaml

# Apply Persistent Volume Claim (PVC)
kubectl apply -f k8s/postgres-pvc.yaml

# Apply Deployments and Services
kubectl apply -f k8s/postgres-deployment.yaml
kubectl apply -f k8s/pgadmin-deployment.yaml
kubectl apply -f k8s/web-deployment.yaml

# Apply Ingress
kubectl apply -f k8s/ingress-service.yaml

echo "Deployment completed successfully."

# run ./deploy.sh
