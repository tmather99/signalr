TAG             = v1003
ENV             = staging
PROJ            = blue
REPO            = harbor-repo.vmware.com
HARBOR_PROJ     = $(REPO)/jedi
REPO_TAG        = projects.registry.vmware.com
HARBOR_PROJ_TAG = $(REPO_TAG)/jedi

.PHONY: ingress signalr

#
# build targets
#
build:
	docker build -t $(HARBOR_PROJ)/signalr:$(TAG) -f  ./signalr/Dockerfile .

tag:
	docker tag $(HARBOR_PROJ)/signalr:$(TAG) $(HARBOR_PROJ_TAG)/signalr:$(TAG)

push:
	docker push $(HARBOR_PROJ)/signalr:$(TAG)
	docker push $(HARBOR_PROJ_TAG)/signalr:$(TAG)

run:
	docker run -it --rm --name signalr -p 80:80 -e "ASPNETCORE_ENVIRONMENT=Development" $(HARBOR_PROJ)/signalr:$(TAG)

client:
	C:/github/ps-globomantics-signalr-itemgroups/ConsoleClient/bin/Debug/net7.0/ConsoleClient.exe http://localhost

client-blue:
	C:/github/ps-globomantics-signalr-itemgroups/ConsoleClient/bin/Debug/net7.0/ConsoleClient.exe https://signalr-staging-blue.assistdevtest.com

login:
	docker login $(REPO)

clean:
	docker system prune -f
	docker volume prune -f

#
# cluster targets
#
cluster:
	kind create cluster --name cluster --config=kind/cluster.yml

rm-cluster:
	kind delete cluster --name cluster

#
# ingress targets
#

# https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/kind/deploy.yaml
nginx-kind:
	kubectl apply -f ingress/nginx-controller-kind-v1.8.1.yml

rm-nginx-kind:
	kubectl delete -f ingress/nginx-controller-kind-v1.8.1.yml
	
secrets:
	kubectl apply -f ingress/tls-secrets/signalr-assistdevops-tls.yml	
	kubectl apply -f ingress/tls-secrets/seq-assistdevops-tls.yml	

rm-secrets:
	kubectl delete -f ingress/tls-secrets/signalr-assistdevops-tls.yml	
	kubectl delete -f ingress/tls-secrets/seq-assistdevops-tls.yml	

ingress:
	kubectl apply -f ingress/seq.yml	
	kubectl apply -f ingress/signalr.yml	

rm-ingress:
	kubectl delete -f ingress/seq.yml	
	kubectl delete -f ingress/signalr.yml	

seq:
	kubectl apply -f deploy/seq.yml

rm-seq:
	kubectl delete -f deploy/seq.yml

signalr:
	kubectl apply -f deploy/signalr.yml	
	kubectl apply -f deploy/signalr-1.yml	
	kubectl apply -f deploy/signalr-2.yml	

rm-signalr:
	kubectl delete -f deploy/signalr.yml	
	kubectl delete -f deploy/signalr-1.yml	
	kubectl delete -f deploy/signalr-2.yml	