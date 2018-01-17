

Events/Emitters/Observers

Most entities in the game are Emitters, i.e. will emit signals (or events) that Observers will
pick up, and appropriately respond to.

Many GameObjects are Observers, and are linked (attached or registered) with an Emitter, all will
respond to events fired by the Emitter. Observers respond to events via the HandleEvent override.

Furthermore, Emitters can have Actions attached to it. Actions are like observers, except they are
not really classes but Functors (or Closures), that respond (get called) to a single event.

Event names can be anything. An event that is fired in response to a property change should be called
"On<PropertyName>Changed".