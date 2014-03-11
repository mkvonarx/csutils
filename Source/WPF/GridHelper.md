# GridHelper

I've implemented a little WPF utility class that I'd like to share: GridHelper. GridHelper is a WPF utility class that provides two attached properties for defining the rows and columns of a WPF Grid layout control. What is it good for? Read on.


## Problem

I love WPF, but I hate to enter endless X(A)ML just to define the rows and columns of a WPF Grid layout. There really should be a simpler way, but obviously there still isn't any even with WPF 4. Here's what I'm talking about:

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

Isn't this pathetic...?


##Goal

All that XAML actually doesn't hold a lot of information. The rows and the columns data could easily be encoded each into a simple XML attribute directly on the Grid XML element. Like this:

```XAML
<Grid utils:GridHelper.Rows="Auto,*,100" utils:GridHelper.Columns="Auto(min=100),*">
    ...
</Grid>
```

Isn't this much more elegant? I certainly think so!


## Solution

The implementation is actually quite simple. Two simple attached properties is all that is needed to implement this functionality. Voilà the "GridHelper" utility class. I was surprised it is so easy to implement. And it even works with the Visual Studio WPF designer. So no reason to code the ugly XAML bloat for Grid layouts anymore.


## Documentation

Read the source code ;-) Or the following short summary.

Format of attributes:

* comma separated list of rows/columns: GridHelper.Rows="ROW1,ROW2,ROW3,..."
* possible values: "*", "Auto" or number of pixels: GridHelper.Rows="*,Auto,200"
* options can be provided in parenthesis: GridHelper.Rows="ROW1(OPTIONS1),ROW2(OPTIONS2),ROW3(OPTIONS3),..."
* possible options: min/max limits: GridHelper.Rows="*(Min=100,Max=200),Auto(Min=100),200"

Samples:

* GridHelper.Rows="Auto,*,200"
* GridHelper.Columns="Auto(max=500),*(min=300),100"


##Implementation & Licence

Feel free to download the provided sample implementation. I provide it under the 3-clause BSD license for free. No guarantees of any kind! Use on your own risk! It would be nice to hear from anybody being happy with my source code.
