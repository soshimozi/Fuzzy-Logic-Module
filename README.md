![license](https://img.shields.io/npm/l/ngclipboard.svg)

# Fuzzy Logic Module
###### A C# library to implement fuzzy logic

What is fuzzy logic?  We are all familiar with the normal boolean logic that is implemented using a true/false mechanism.  
Statements in boolean logic can either be true or they can be false, but nothing inbetween.  This sort of logic works well with 
computers since computers are operated, at the very basic level, by switches.  A switch can either be on, or it can be off, it can't 
be inbetween.  Boolean logic fits well for that.

In the real world, however, there are many cases when determining something isn't so cut and dry.  In nature it's very hard to find 
systems or mechanisms that are always true or always false given the same inputs.  Many times it's difficult to model these natural 
phenomenom using a true/false mechanism or boolean logic.  

A simple example is when trying to make a decision if you need to stop for gas before driving home.  You take into account many of 
the current conditions, such as weather, driving conditions, traffic, and so on, as well as the amount of gas you have left coupled 
with your driving style before you make your decision.  Maybe it's raining and you are a hard driver and the nearest gas station once 
you start your trip is very far away.  

Again, the output from that "function" won't be a cut and dry "yes" or "no", there will be degrees of yes and no.  What are called 
degrees of membership.  Maybe there are more degrees of membership in "yes" than in "no" which would result in a decision that favors 
the "yes" condition.

## Examples

###### Decorating your POCO
```
    public class Enemy : DynamicPOCO
    {
        public double Health { get; set; }

        public int Age { get; set; }
        
        public double Hunger { get; set; }

        [Observable]
        public double DistanceToTarget { get; set; }

        [Observable]
        public int AmmoStatus { get; set; }

        [Observable]
        public double Skill { get; set; }

        public double Desirability { get; set; }
    }
```

###### Wrapping an object
```
var module = new FuzzyModule();

 var enemyWrapped = new Enemy();
 var fo = new ObservableFuzzyObject<Enemy>(enemyWrapped, module);

 // Now you can call either the enemyWrapped or the proxy, both will result in the same:
 fo.Proxy.DistanceToTarget = 23;

 Assert(enemyWrapped.DistanceToTarget == 23);
```

###### Defining some terms (terms are used in rules)
```
fo.DefineFuzzyTerm("Very_Skilled", p => p.Skill, new RightShoulderFuzzySet(20, 100, 80));
fo.DefineFuzzyTerm("Skilled", p => p.Skill, new TriangleFuzzySet(10, 30, 20));
fo.DefineFuzzyTerm("Low_Skilled", p => p.Skill, new TriangleFuzzySet(0, 20, 5));

fo.DefineFuzzyTerm("Ammo_Loads", p => p.AmmoStatus, new RightShoulderFuzzySet(10, 100, 20));
fo.DefineFuzzyTerm("Ammo_Okay", p => p.AmmoStatus, new TriangleFuzzySet(0, 30, 10));
fo.DefineFuzzyTerm("Ammo_Low", p => p.AmmoStatus, new TriangleFuzzySet(0, 10, 0));

fo.DefineFuzzyTerm("Undesirable", p => p.Desirability, new LeftShoulderFuzzySet(0, 50, 25));
fo.DefineFuzzyTerm("Desirable", p => p.Desirability, new TriangleFuzzySet(25, 75, 50));
fo.DefineFuzzyTerm("VeryDesirable", p => p.Desirability, new RightShoulderFuzzySet(50, 100, 75));

fo.DefineFuzzyTerm("Target_Close", p => p.DistanceToTarget, new LeftShoulderFuzzySet(0, 150, 25));
fo.DefineFuzzyTerm("Target_Medium", p => p.DistanceToTarget, new TriangleFuzzySet(25, 300, 150));
fo.DefineFuzzyTerm("Target_Far", p => p.DistanceToTarget, new RightShoulderFuzzySet( 150, 1000, 300));

fo.DefineFuzzyTerm("Undesirable", p => p.Desirability, new LeftShoulderFuzzySet(0, 50, 25));
fo.DefineFuzzyTerm("Desirable", p => p.Desirability, new TriangleFuzzySet(25, 75, 50));
fo.DefineFuzzyTerm("VeryDesirable", p => p.Desirability, new RightShoulderFuzzySet(50, 100, 75));

```
By defining a term you are telling the fuzzy logic module that when referencing that term it should use the resulting shape to determine the degree of membership in the set.  The term will associate a property of the object as input to the shape to determine the overall degree of membership.  Terms are used exclusively in rules as the following examples illustrate.

###### Defining some rules

Using the expression syntax
```
module.AddRule("IF VERY(DistanceToTarget:Target_Far) THEN Desirability:VeryDesirable");
```

Using verbose syntax
```
module.AddRule(
    FuzzyOperator.Or(FuzzyOperator.And(fo["Target_Close"], fo["Ammo_Low"]),
           FuzzyOperator.Or(FuzzyOperator.And(fo["Target_Close"], fo["Ammo_Loads"]),
           FuzzyOperator.And(fo["Target_Close"], fo["Ammo_Okay"]))), fo["Undesirable"]);

```
The above is equavalent to the statement ```IF (DistanceToTarget:Target_Close AND AmmoStatus:Ammo_Low) OR (DistanceToTarget:Target_Close AND AmmoStatus:Ammo_Loads) OR (DistanceToTarget:TargetClose AND AmmoStatus:Ammo_Okay) THEN Desirability:Undesirable.```

You can see that the expression syntax is easier, but the verbose syntax is more powerful

###### Resolving values
In order to resolve values you must "defuzzify" the values using the Defuzzify method.

```
 fo.DeFuzzify(p => p.Desirability, m => m.DeFuzzifyMaxAv());
 ```
 The above code will resolve the "fuzzy" input values and provide a concrete value on the output.  The value will be applied to the property in the first argument using the method in the second argument.  There are two methods available: MaxAverage and Centroid.
 
###### Shapes

