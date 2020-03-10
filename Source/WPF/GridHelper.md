# GridHelper

GridHelper is a WPF utility class that provides two attached properties for defining the rows and columns of a WPF Grid layout control.


## Problem

I don't like the endless XAML boilerplate that is required in WPF just to define the rows and columns of a WPF Grid layout. There really should be a simpler way, but unfortunately there isn't. Here's an example of such boilerplate XAML:

```XAML
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
        <RowDefinition Height="100" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" MinWidth="100" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>
    ...
</Grid>
```


## Goal

That boilerplate XAML actually doesn't hold a lot of information. The row and the column definitions could easily be encoded in two simple XML attributes directly on the Grid XML element. Like this:

```XAML
<Grid utils:GridHelper.Rows="Auto,*,100" utils:GridHelper.Columns="Auto(min=100),*">
    ...
</Grid>
```


## Solution

Two simple attached properties is all that is needed to implement this functionality. It even works with the Visual Studio WPF designer. So no reason to code the ugly XAML boileplate bloat for Grid layouts anymore.


## Documentation

Format of attributes:

* comma separated list of rows/columns:                                      `GridHelper.Rows="ROW1,ROW2,ROW3,..."`
* supported size values:                  `"*"`, `"2*"`, `"Auto"` or `200`:  `GridHelper.Rows="*,Auto,200,2*"`
* options can be provided in parenthesis:                                    `GridHelper.Rows="ROW1(OPTIONS1),ROW2(OPTIONS2),ROW3(OPTIONS3),..."`
* supported options:                      min/max limits, shared size group: `GridHelper.Rows="*(Min=100,Max=200),Auto(Min=100),200,Auto(SharedSizeGroup=abc)"`

Samples:

* `GridHelper.Rows="Auto,*,200,2*"`
* `GridHelper.Columns="Auto(max=500),*(min=300),100"`


## Implementation & Licence

The source code is available here: https://github.com/mkvonarx/mkvonarx-csutils/blob/master/Source/WPF/GridHelper.cs

The code and samples are licences under the 3-clause BSD license.
