# distrib
A very complex, no longer maintained attempt at building a distributed job-based processing system

## Motivation
A product I worked on several years ago at a past employer used a "distributed" process model for offloading certain tasks like communication / file system access etc, except it didn't really work entirely in practice.

I wanted to come up with a way of being able to share assemblies between nodes, where a plugin-based system could bootstrap hosts for particular job processes that could be remotely invoked.

## Current State
It's massively unfinished, it has the process loading and execution stuff in there, as well as some RPC stuff I worked on in a different project; there's even some early ported work on auto-discovery of data sources based on tags, so processes could communicate data without direct knowledge of the data structure (a mapper would try to work this out).

## License
The code and the such says it's all GPL, but that was way back when, I'm beyond that now, this README gives you permission to make use all code in this repo (where I can do so) under the MIT license.
