FROM alpine

EXPOSE 5004
COPY out app

RUN apk add --no-cache libgcc libstdc++ icu-libs && \
	adduser -DH dotnet

USER dotnet
WORKDIR /app

CMD ["/app/Orderpicker"]