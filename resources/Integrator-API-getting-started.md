# Getting started with dol-sensors IoT integration 

 
# High level concepts

The dol-sensors integration is split in two parts.
- Http API
- Service bus queue subscriptions 

## Http API

The http API is where you register new devices, configure and update them and tell devices to run certain commands. All this is initiated from your side, either by the end-user or by your overall system. 

All incoming data measurements and status from the registered devices is **NOT** found in the http API.

## Queues

All device data and status messages is placed in queues, from where you can read them as they arrive. This includes both the measurements from devices, as well as any updates to the status of the devices. 

## Integrators

In order to integrate to dol-sensors iot devices, you need an integrator account.
This in turn gives you access to the HTTP API and the data and status queues that contains the live data from devices.

You only need one integrator account, not one for each user you have. The integrator account is *you (or more likely your company)*.
The individual end-users that exists in your software / UI is not represented in the dol-sensors api. 

## Logins

To communicate with the API, you need a login. 
- A login is attached to an Integrator account. 
- An integrator account can have multiple logins. 

The login is the way you are authorized to perform actions on the API "on behalf of" the integrator account.

## Device

Devices are the actual IoT devices sold by dol-sensors. 

### IDOL64 - wireless gateway

The IDOL64 is a wireless gateway for LoRa enabled sensors.
This device will handle the wireless communication with the different LoRa sensors.  
It is the different sensors that performs the measurements (temperature, co2, humidity, ammonia, etc..)

Besides the up to 50 wireless sensors, the IDOL64 also allows 4 wired sensors to be connected. 

### IDOL65 - pig weighing camera

The IDOL65 device is camera mounted above a pig pen that will measure the pig's weight. In contrast to the IDOL64 device, the camera does not have any additional sensors.  

## Sensors

Sensors are what actually makes the different measurements. Typically these sensors does not have an internet connection, and thus it has no way of delivering measurements to the cloud. 
Instead they talk to a gateway device like IDOL64 that will upload the data measurements for the sensor. 

Examples of sensors:
- DOL 16  - Light sensor (measurements in LUX)
- DOL 53 - Ammonia Sensor (measurements in NH3 - parts per million)
- IDOL 139 - Temperature, humidity and CO2 (3 different measurements, one sensor)  

# Quick start

For a nice interactive view of all the endpoints in swagger - [click here](https://iot.dol-sensors.com/swagger/index.html) or [here for test environment](https://dol-iot-api-qa.azurewebsites.net/swagger/index.html). 

## Integrators and logins

First, we need to register a new **login** to start using the API. 

`POST /api/auth/register` to register an account.
```json
{
  "email": "mic@dol-sensors.com",
  "password": "SomeLongPassword123!"
}
```

You will then receive an email where you can confirm that it is indeed your email.

Once a a login has been created, we can attach the login to an **integrator** account. 
**To create a new integrator and attach the login, a dol-sensors employee is required.** 

With our new login now attached to an integrator, we can start using the API.

`POST /api/auth/login` to receive a bearer token.
```json
{
  "email": "mic@dol-sensors.com",
  "password": "SomeLongPassword123!"
}
```

response looks like this

```json
{
  "tokenType": "Bearer",
  "accessToken": "............................",
  "expiresIn": 3600,
  "refreshToken": "............................"
}
```

To authenticate our requests we use the accessToken as a bearer token in our http request headers.

```curl 
curl --header 'Authorization: Bearer {accessToken}'
```

We can use this accessToken for one hour. After that it will expire.

To refresh the accessToken we use the `/api/auth/refresh` endpoint with our refresh token. With that, we receive a new accessToken (valid for another hour) and a new refreshToken.  

## Register new device (Claim device)

All endpoints (except auth) require login and that our login is attached to an integrator.
To register a new device, we "claim" it as ours. This means our integrator now "owns" this device. Data and status from the device will start being forwarded to the integrator queues.

`POST /api/devices/claim`
```json
{
  "macAddress": "00abcd1234ef", // this is written on the actual physical device
  "key": "someKey", // also written on the device
  "deviceType": "IDOL64", // the type of dol-sensors device
  "owner": "Optional", // use this only if you want some identifying information about the device saved in dol-sensors system (like a customer id or similar)
  "deviceName": "Optional" // use this only if you want some identifying information about the device saved in dol-sensors system
}
```

If you get an 200 OK back, the device is now claimed by your "integrator". 

## View devices

To get an overview of all our (integrators) devices call

`GET /api/devices` 
```json
{
  "devices": [
    {
      "mac": "00abcd1234ef",
      "deviceName": "some name",
      "deviceType": "IDOL64",
      "createdAt": "2023-11-23T14:54:30Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 100,
  "deviceTotal": 1
}
```

Use the optional query parameters page and pageSize to manipulate paginated response of devices. 
Use the optional owner query parameter to filter devices by owner. 

To get more information about a given device call

`GET /api/devices/{mac}`
```json
{
  "mac": "00abcd1234ef",
  "key": "UvtLPSNd",
  "deviceType": "IDOL64",
  "owner": "optional owner", 
  "deviceName": "optional name", // will default to {deviceType}-{mac}
  "createdAt": "2023-11-23T14:54:30Z",
  "updatedAt": "2023-11-23T14:54:30Z",
  "connectionState": "Connected",
  "isOnline": true,
  "lastActivityUtc": "2024-03-15T10:28:49Z", 
  "cloudToDeviceMessages": 0,
  "sensors": [
    {
      "devEui": "a2..............",
      "name": "Pen 3",
      "createdAt": "2024-02-10T08:10:42Z",
      "latestDataSentAt": "2024-03-15T10:32:15Z",
      "sensorType": "iDOL139",
      "sampleRate": 180
    },
    {
      "devEui": "a3..............",
      "name": "Pen 6",
      "createdAt": "2024-02-10T08:12:42Z",
      "latestDataSentAt": "2024-03-15T10:32:38Z",
      "sensorType": "iDOL139",
      "sampleRate": 180
    },
    {
      "devEui": "a4..............",
      "name": "Outside 1",
      "createdAt": "2024-02-10T08:16:42Z",
      "latestDataSentAt": "2024-03-15T10:32:48Z",
      "sensorType": "iDOL139",
      "sampleRate": 180
    }
  ],
  "wiredSensors": null,
  "cameraStatus": null
}
```

The "sensors" array contains the configued wireless sensors, the "wiredSensors" contains the configured wired sensors.
For IDOL65 these will be null and instead the "cameraStatus" will be filled like so

```json
{
	"cameraDirty": "Clean",
    "manuallyCalibrated": false,
    "calibrationStatus": "Done",
    "dirtyDetectionEnabled": true,
    "lastCalibrationTime": "2024-03-14T13:29:54Z",
    "messages": [
	    {
	      "MessageId": 0,
	      "MessageText": "In-pen calibration successful ",
	      "MessagePayload": ""
	    }
    ]
  }
```

## Add new sensor to device

This operation is only supported by our IDOL64 variants.

To configure a new lora enabled sensor on our device we can call the following endpoint. 
This example is for a DOL53 - an ammonia sensor. 

`POST /api/devices/{mac}/sensor`
```json
{
  "devEUI": "..........", // found on label on the sensor
  "name": "string", // some identifying name so the sensor can easily be identified 
  "type": "DOL53",
  "sampleRate": 600, // the sensors sample rate in seconds
  "sensorDetailsRequest": { // all this is optional
    "productName": "Optional",
    "productionVersion": "Optional",
    "serialNumber": "Optional"
  }
}
```

On a 200 OK the sensor is added to the device.
We can verify that the sensor has indeed been added to the device by calling the device information endpoint again. 
Verify by showing the device details from 

`GET /api/devices/00abcd1234ef`
```json
{
  "mac": "00abcd1234ef",
  "key": "UvtLPSNd",
  "deviceType": "IDOL64",
  "owner": "optional owner",
  "deviceName": "optional name", // will default to {deviceType}-{mac}
  "createdAt": "2023-11-23T14:54:30Z",
  "updatedAt": "2023-11-23T14:54:30Z",
  "connectionState": "Connected",
  "lastActivityUtc": "2023-11-25T14:17:43Z",
  "cloudToDeviceMessages": 0,
  "sensors": [{
      "devEui": "...........",
      "name": "dol53 outside",
      "createdAt": "2023-11-25T08:02:25Z",
      "sensorType": "DOL53",
      "sampleRate": 600,
      "batteryStatus": {
		"code": 0,
		"value": "OK"
      }
  }],
  "wiredSensors": []
}
```

## Add wired sensor to device

IDOL64 devices support up to 4 wired sensors to be connected. The device needs to know which sensors are wired to which port in order to read the data.
We can do this configuration with the following endpoint

`PUT /api/devices/{mac}/wiredSensor`
```json
{
  "sensors": [
    {
      "port": 1,
      "wiredSensorType": "DOL16",
      "samplingRate": 60
    },
    {
      "port": 2,
      "wiredSensorType": "DOL139",
      "samplingRate": 60
    }
  ]
```
The ports 1, 2, 3 and 4 are available for configuration. 
Like the PUT operation suggests, this endpoint will override the current configuration with the new configuration from the request.

Once again you can verify with the `GET /api/devices/{mac}` endpoint


##  Getting data

When the integrator account was created, 2 new exclusive (service-bus) queues was created for our integrator. One for data messages and one for status updates from the devices. These are to two main integration points and where you will receive all the actual device data. 

To read from this service bus queue, you can use a client library in either javascript/typescript, python, dotnet or java. See [link to learn more](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview#client-libraries). 
More languages are supported (C, C++, Go, Ruby, PHP), as long as we can find a AMQP 1.0 protocol client. 

To start subscribing to the data our claimed devices produce, we can get the details of the queues made for our integrator by calling  

`GET /api/management/queue`
```json
{
  "dataQueueConnection": "...................",
  "dataQueueName": "data queue name",
  "dataUsingPrimaryKey": true,
  "statusQueueConnection": "....................",
  "statusQueueName": "status queue name",
  "statusUsingPrimaryKey": true,
  "dataQueueMessageCount": 0,
  "dataQueueDeadLetterCount": 0,
  "statusQueueMessageCount": 0,
  "statusQueueDeadLetterCount": 0
}
```

### Data messages

Take the i.e. "dataQueueConnection" and use that to subscribe to the queue. 

The datamessages has the following format
```json
{
  "id": "54b28108-cd43-48bb-9661-fe24e623a978",
  "deviceId": "00abcd1234ef",
  "sensorId": ".............", // the sensors devEui
  "sensorName": "dol53 outside",
  "value": 2.2, // the actual measurement 
  "type": "Ammonia",
  "unit": "ppm", // unit will tell you what the data in 'value' is. 
  "timestamp": 1700572959 // unix time stamp in seconds
}
```

For IDOL65 (pig weighing cameras) -  the messages have some additional properties. 
Example 
```json
{
  "id": "9e0dccd4-e17e-4451-8505-9c6a111c6ce6",
  "count": 24934, // total weight calculations last 24h 
  "minWeight": 92.8,
  "maxWeight": 122.67,
  "timespan": 3600, // seconds since last weight update
  "sd": 7.47, // standard deviation
  "skewness": 2.69,
  "lastCycleCount": 401, // weight calculations for the last timespan
  "lastCycleMeanWeight": 108.21, // mean weight over last timespan,
  "lastCycleMinWeight": 93.1,
  "lastCycleMaxWeight": 123.55,
  "lastCycleSD": 8.12,
  "lastCycleSkewness": 2.12,
  "deviceId": "ddeecdff015f",
  "sensorId": "ddeecdff015f",
  "sensorName": "some sensor name",
  "value": 107.73, // the mean weight over the last 24h
  "type": "Weight",
  "unit": "kg",
  "timestamp": 1700572959 // unix time stamp in seconds
}
```

### Device status messages

When some status changes on the device that the end user could be interested in, a new message will be put into the status queue.

To differentiate between the different messages, a Subject (sometimes called a label) is set on the message.

These are the message types at the moment.

#### Subject/label = "DeviceConnectionChanged"

Will notify on a connection change for the device. Can be either deviceConnected or deviceDisconnected.

```json
{
    "deviceId": "aabbccddeeff",
    "state": "deviceConnected", // or "deviceDisconnected"
    "timestamp": 1700151990
}
```

#### Subject/label = "SensorsInactive"

For the IDOL64 device. If one or more configued lora sensors no longer sends data it will be reported with this message. The device will allow 2 * (sensors configured sample time) seconds to go by without hearing anything, before the sensor is reported as inactive.  

If a sensor starts sending data again, a new "SensorsInactive" message will get sent, where the device is removed from the inactiveSensors list.
```json
{
  "deviceId": "aabbccddeeff",
  "inactiveSensors": [
    {
      "name": "Environment sensor",
      "devEui": "f2b3d57fbbbb2304",
      "lastSeenAt": "2023-11-27T12:35:26Z"
    },
    {
      "name": "Ammonia sensor",
      "devEui": "a81758fbbbbb55ee",
      "lastSeenAt": "2023-11-27T10:35:26Z"
    }
  ],
  "timestamp": 1700151990
}
```

#### Subject/label = "VisionStatus"

For IDOL65 devices. 
Will report any changes to the status of the camera.
```json
{
    "deviceId": "aabbccddeeff",
    "isDirty": "Clean",
    "calibration": "Done", 
    "messages": [
	    {
	      "MessageId": 0,
	      "MessageText": "In-pen calibration successful ",
	      "MessagePayload": ""
	    }
    ],
    "timestamp": 1700151990
```

### Subject/label = "SensorBatteryUpdates"

Our sensors powered by elsys lora module uses a battery. 
This status update will be sent if any sensors battery state has changed.
The battery will be in OK state for most of its life time 

The states are 
code 0 = "OK", 
code 1 = "Low", 
code 2 = "Very low" 
code 3 = "Critical".

```json
{
  "deviceId": "000ecd02c131",
  "timestamp": 1713437768,
  "batteryUpdates": [
    {
      "devEui": "a81758fffe0b55ee",
      "code": 2,
      "batteryStatus": "Very low"
    }
  ]
}
```
