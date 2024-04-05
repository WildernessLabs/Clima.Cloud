# Clima.Cloud

An example back-end enterprise application for consuming and displaying IoT data collected from Wilderness Labs Clima devices.

## Requirements/User Stories

- Any Clima device can opt-in to send data to Clima.Cloud
- Clima device locations are regionally anonymized to a postal code - exact locations are not stored or displayed
- A global map is presented to users showing all know Clima devices
  - Each device is color coded for age of latest data (e.g. maybe green == "active in last hour", yellow == "active last 24 hours" and red == "data > 24 hours stale" or similar)
  - Each device shows wind speed/direction and temp at a glance
  - Each device can be clicked on to show more detail of latest record (temp, wind, pressure, humidity, etc)
  - Each device can be selected and show a history (e.g. maybe last 48 hours or similar)
 
## Data Model

TBD
