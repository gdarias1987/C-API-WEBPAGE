API - Examen MercadoLibre

Funciones: 
	- Se deberá generar un token JWT para realizar las operaciones de la API
	
	- Se podrá crear eventos con su estado inicial en "Handling" y subestado en "null". 
	
	- Se podrá recibir un paquete de checkpoint asincrónicos, y actualizar los checkpoints por su ID en su último estado lógico. 
	
	- Se podrá consultar la información de un evento especifico mediante su ID.
	
	- Se podrán visualizar todas las funciones mediante la UI desarrollada para la página inicial.

__________________________________________________________________________________________________________________
Modelo de datos:

Evento:
-------------------------------------------
	- id (PK - integer - not null)
	- idEvento (Unique - integer - not null)
	- estado (string - not null)
	- subestado (string - nullable)
	- timestamp (string - not null)

Usuario
-------------------------------------------
	- idUsuario (integer - not null)
	- usuario (string - not null)
	- pass (string - not null)
__________________________________________________________________________________________________________________

JWT LOGIN
____________

 Method: POST -> /api/user/jwt
 Función: GenToken 
 Descripción: Esta función permite obtener un token para realizar las operaciones en la API. Se requieren las credenciales de usuario en el cuerpo, en formato JSON. El token tiene validez por 24 horas.
					El token ingresado deberá estar en la cabecera del request de la operación a realizar, con el key Autorization y en el valor el token.

 Json entrada -> 
	{
		"usuario":  string usuario,
		"pass":  string password
	}

 Json salida -> 
 	Success 
	---------
	( HTTP CODE 200 )
	{
		"token": string token
	}
		
	Error
	------
		( HTTP CODE 401 - Unauthorized)
				{
			"type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			"title": "One or more validation errors occurred.",
			"status": 401,
			"traceId": "|f8842084-4ffc3bdba91ee278."
		}
__________________________________________________________________________________________________________________

CHECKPOINTS
____________

 Method: GET  -> api/checkpoint/
 Función: getAllCheckpoints 
 Descripción: Esta función permite obtener como resultado un array en formato JSON con los eventos.

 Json entrada -> Null
 
 Json salida -> 
	
	Success 
	---------
	( HTTP CODE 200 )
		[
			{
				"idEvento": int id,
				"estado": string estado,
				"subestado": nullable string subestado,
				"timestamp": string timestamp
			},
			.
			.
			.
		]
______________________________________
 
 Method: GET -> api/checkpoint/ID
 Función: getCheckpointByID
 Descripción: Esta función permite obtener un Evento especifico mediante su ID. El atributo "ID" en el url, debe ser un entero.
 
 Json entrada -> Null
 Json salida -> 
	
	Success 
	---------
	( HTTP CODE 200 )
		{
			"idEvento": int id,
			"estado": string estado,
			"subestado": nullable string subestado,
			"timestamp": string timestamp
		}
	
	Error 
	------
	( HTTP CODE 404 - Not found - El "ID" inexistente)
		{
			"type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			"title": "One or more validation errors occurred.",
			"status": 400,
			"traceId": "|f8842084-4ffc3bdba91ee278.",
			"errors": {
				"id": [
					"The value 'a' is not valid."
				]
			}
		}
		
	( HTTP CODE 400 - Bad request - El "ID" ingresado es incorrecto )
		{
			"type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			"title": "One or more validation errors occurred.",
			"status": 400,
			"traceId": "|f8842084-4ffc3bdba91ee278.",
			"errors": {
				"id": [
					"The value 'a' is not valid."
				]
			}
		}
 
______________________________________	
 
 Method: POST -> api/checkpoint/ID
 Función: createCheckpoint
 Descripción: Esta Función permite generar un nuevo Evento, indicando en el atributo "ID", el numero de evento.
 
 Json entrada -> Null
 Json salida -> 
	
	Success
	---------
	( HTTP CODE 201 -Created  )
		{
			"idEvento": int id,
			"estado": string estado,
			"subestado": nullable string subestado,
			"timestamp": string timestamp
		}
	
	Error 
	------
	( HTTP CODE 409 - Conflict - El "ID" ingresado ya existe )
		{
			"type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			"title": "One or more validation errors occurred.",
			"status": 400,
			"traceId": "|f8842084-4ffc3bdba91ee278.",
			"errors": {
				"id": [
					"The value 'a' is not valid."
				]
			}
		}

	( HTTP CODE 400 - Bad request - El "ID" ingresado es incorrecto )
		{
			"type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			"title": "One or more validation errors occurred.",
			"status": 400,
			"traceId": "|f8842084-4ffc3bdba91ee278.",
			"errors": {
				"id": [
					"The value 'a' is not valid."
				]
			}
		}
 
		
	
______________________________________
 Method: PUT -> api/checkpoint/
 Función: handleEnvio
 Descripción: Esta función permite recibir un conjunto de checkpoints, y actualizar los estados/subestados correspondientes. 
	
 Json entrada -> 
 	
	[
		{
			"idEvento": int id,
			"estado": string estado,
			"subestado": nullable string subestado
		},
		.
		.
		.
	]
	
 Json salida ->
 
	Success
	---------
	( HTTP CODE 200 )
		[
			{
				"idEvento": int id,
				"estado": string estado,
				"subestado": nullable string subestado,
				"timestamp": string timestamp
			},
			.
			.
			.
		]
	
	Error
	------
	( HTTP CODE 400 - Bad request - El "ID" ingresado es incorrecto )
		{
			"type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
			"title": "One or more validation errors occurred.",
			"status": 400,
			"traceId": "|a24f3adf-47feef29c262edc0.",
			"errors": {
				"$[0].idEvento": [
					"The JSON value could not be converted to System.Int32. Path: $[0].idEvento | LineNumber: 1 | BytePositionInLine: 21."
				]
			}
		}

