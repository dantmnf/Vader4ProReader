# Vader 4 Pro Reader

This is a prototype application for accessing full functionality of the Flydigi Vader 4 Pro controller. It is meant to help developers add support for this controller to their application.

![vader4proreader](https://github.com/user-attachments/assets/04e5ed18-9c75-4e19-95ea-c4620175a9af)


## Known Issues

* Only wired/dongle connection with DInput mode (FN+A, blue LED) is supported.
  - Other modes (Bluetooth/Switch/XInput) do not give all buttons and axes.
* The range of gyroscope reading (angular velocity in SI) is unknown.
* Accelerometer readings automatically scale to `(0, 256, 0)` on idle, scaled again with constant `9.80665/256`.
  - May lead to inaccurate readings on free fall
* No battery level reporting.
