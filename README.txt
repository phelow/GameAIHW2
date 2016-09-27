1)	What did you do for avoiding a group of agents? What are the weights of path following and evade behavior?
		- For the first Cone Check implementation a center of mass was calculated based off the average
			of all of the nearby agents. This proved to be less effective than only avoiding
			the closest agent.
		-Cone Check closest: Avoid: .7, Follow: .3
		-Cone Check mass: Avoid: .8, Follow: .2
		-Collision Prediction Check mass: Avoid: .7, Follow: .3


2)	What are the differences in cone check and collision prediction’s performances?
		- collision prediction appeared to do noticeably better than cone check. It seemed that
			avoiding the position of the enemy when it would be most close to you was more
			effective than avoiding it's current position.
		