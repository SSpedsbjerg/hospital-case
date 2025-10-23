# hospital-case

## Analysis of the code

First I look through all of the files and notes down all thoughts. First I notice immediately that one can reduce the code using a dictionary.

Using Postman I can test the intended behaviour which I will have to refactor.

I notice here that a stacktrace is given even when running a release build, I then notice in the config it is set to do that.

Also I would assume that if someone already have a valid Referral, they have also aquired one. Therefore if HasValid methods will also call Requires to reduce possible confusion.

## Task

While a dictionary can further increase maintainability, I also want to go further in that aspect. Here I think that using the Func<> type could go a long way, but the problem is that the parameters are already set and I do not want to change already existing method parameters. Therefore Delegate type is the option to go forward with. It lets the compiler handle the types, parameters in runtime which do make it a bit harder to ensure the state of the type, but I believe the context of this task it is the best option forward.

## Furture Work

I also know that you can reference methods in a configuration file and load that up, thereby entirely seperating the concerns of the AppointmentService from creating its own possible departments. Meaning that everything in the constructor could be moved to a config file. I will not do this as the case states that I should not spend more than a few hours on the task, and adding this in would take me above that threshold.
