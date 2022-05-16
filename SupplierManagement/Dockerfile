FROM alpine

EXPOSE 5007
COPY out app

RUN apk add --no-cache libgcc libstdc++ icu-libs && \
	adduser -DH dotnet

USER dotnet
WORKDIR /app

CMD ["/app/SupplierManagement"]