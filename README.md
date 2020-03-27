# Synth1PresetGen

This project aims to create Synth1(Vsti) Presets from an existing audio-sample to get the nearest possible sound out of the VSTi. 

#### The project is divided into two main parts:

* TestSampleGen - A tool that creates audio samples for testing from the Vst synthesizer Synth1
* Machine learning..... coming soon!

## Getting Started

Clone project or download and extract zip-file.

### Prerequisites

* Installed Visual Studio 

### Installing

TestSampleGen:

Open TestSampleGen/TestSampleGen.sln in Visual Studio. 
Install NuGet packages VST.NETx86 und NAudio. At the time of writing the version of both was v1.10 but I except no issues if newer versions come up.

### Usage

TestSampleGen:

At the moment you have to set up Synth1 once for the tool to work:

* Run TestSampleGen inside Visual Studio
* Press the load Vst Button
* Select in the Synth1 folder the file  'Synth1 VST.dll'


## Running the tests

coming soon!

## Built With

* [VST.NET](https://github.com/obiwanjacobi/vst.net) - The web framework used
* [Visual Studio 2019 community](https://visualstudio.microsoft.com) - Dependency Management


## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Patrick Steurer** - *Initial work* - [GenExe](https://github.com/GenExe)

## License

This project is licensed under the GNU GPL V3 License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc

