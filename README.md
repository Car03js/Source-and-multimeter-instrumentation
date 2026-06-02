# About this project.

This is a school project wher it simulates a IC test plan production line, where the objective is to verify that multiple PCBs meet the electrical specifications defined in the corresponding component datasheets. To accomplish this, the system compares measured values against acceptable ranges stored in a SQL Server database.

The application is built using C# and SQL Server, allowing centralized management of test parameters, storage of test results, and generation of historical reports for each tested board. This enables traceability and facilitates quality control throughout the testing process.

The project also implements Dependency Injection (DI) through constructor injection to improve modularity, maintainability, and testability of the software architecture.

Communication with the test instruments is handled through the NI-VISA API, enabling automated measurement acquisition and instrument control using industry-standard communication protocols.



# Technologies used.

## Hardware Components

The project was developed and tested using the following hardware components:

- 12 VDC Motor
- DPDT Relay
- 1N4007 Protection Diode
- Sealevel I/O Module
- 12 VDC Power Supply
- 24 VDC Power Supply (used to power the Sealevel module)
- 50 kΩ Potentiometer
- Various Resistors
- Various Capacitors
- Protection Diodes

## Software and Technical Foundation

The application was developed using a graphical user interface (GUI) architecture in Windows Forms and follows an object-oriented design approach.

- Development Environment
- Language: C# (.NET 8)
- IDE: Visual Studio 2026
- Paradigm: Object-Oriented Programming (OOP)
- Application Types: Class Libraries, Console Applications, and Windows Forms
- Communication Technologies
- NI-VISA (Ivi.Visa / NationalInstruments.Visa) for instrument communication
- SCPI (Standard Commands for Programmable Instruments) for programmable test equipment control
- Modbus communication for interfacing with the Sealevel module


## Instruments.

- Agilent 34401A Digital Multimeter.
- Agilent 66312A DC Power Supply.
