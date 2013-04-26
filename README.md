# MSC_PLS
A new generation of music player which concentrates on his main function - playing music. But if you need some special extra function, you can simply extend it with a plugin.

**Developers**

* [LinuxDoku](https://github.com/LinuxDoku)
* tr3yf1x


## Background
This app was developed as a project in the software engeneering subject at our vocational school.
It was the first C# .NET we build (we're from other low level programming languages - C, RPG, JavaScript, ...) so please be patient while reading our code, we gave our best is this little time ;-)

## Run
Download this repository, open it with Visual Studio (we used Visual Studio 2012 Ultimate) and compile it. That's all.

* **`msc_pls.exe`** the modern GUI App
* **`msc_pls_l33t.exe`** the console App with the same - err, no - with more functionality

Both are using the same database (library.db - SQLite) for storing your data. The GUI Version only finds Music in USERFOLDER/Music. E.g. C:/Users/LinuxDoku/Music. In the l33t mode you can add directories by yourself (see `help` command).

## ToDo

* finishing wikipedia plugin (look for author information)
* finishing process monitoring plugin (start playlist when you open an application on your host)
* extend ui a bit (deleting of playlists and songs with dragging on recycle bin)
* extend plugin api
* documentation
* ...

## Contribute
If you have some feature or you want to contribute to this project grab the source and do your best. If your modification is against the simplicity thought, we may publish it as an 3rd party plugin.

## Thanks to

* Microsoft for this great framework
* MahApps for the GUI Framework
* Stackoverflow for the great Q&A plattform
* our teacher who made this project possible

## Licence
**MIT**
Copyright (c) 2012 - 2013 LinuxDoku, tr3yf1x
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.