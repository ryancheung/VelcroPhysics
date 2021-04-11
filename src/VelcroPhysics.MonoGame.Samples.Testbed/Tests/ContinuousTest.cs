﻿/*
* Velcro Physics:
* Copyright (c) 2017 Ian Qvist
* 
* Original source Box2D:
* Copyright (c) 2006-2011 Erin Catto http://www.box2d.org 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VelcroPhysics.Collision.Distance;
using VelcroPhysics.Collision.TOI;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.MonoGame.Samples.Testbed.Framework;
using VelcroPhysics.Shared;
using VelcroPhysics.Utilities;

namespace VelcroPhysics.MonoGame.Samples.Testbed.Tests
{
    public class ContinuousTest : Test
    {
        private readonly Body _box;
        private readonly Body _ground;
        private float _angularVelocity;

        private ContinuousTest()
        {
            List<Vertices> list = new List<Vertices>();
            list.Add(PolygonUtils.CreateLine(new Vector2(-10.0f, 0.0f), new Vector2(10.0f, 0.0f)));
            list.Add(PolygonUtils.CreateRectangle(0.2f, 1.0f, new Vector2(0.5f, 1.0f), 0));

            _ground = BodyFactory.CreateCompoundPolygon(World, list, 0);

            _box = BodyFactory.CreateRectangle(World, 4, 0.2f, 1);
            _box.Position = new Vector2(0, 20);
            _box.BodyType = BodyType.Dynamic;

            //_box.Body.Rotation = 0.1f;

            //_angularVelocity = 46.661274f;
            _angularVelocity = Rand.RandomFloat(-50.0f, 50.0f);
            _box.LinearVelocity = new Vector2(0.0f, -100.0f);
            _box.AngularVelocity = _angularVelocity;

            DistanceGJK.GJKCalls = 0;
            DistanceGJK.GJKIters = 0;
            DistanceGJK.GJKMaxIters = 0;
            TimeOfImpact.TOICalls = 0;
            TimeOfImpact.TOIIters = 0;
            TimeOfImpact.TOIRootIters = 0;
            TimeOfImpact.TOIMaxRootIters = 0;
        }

        private void Launch()
        {
            _box.SetTransform(new Vector2(0.0f, 20.0f), 0.0f);
            _angularVelocity = Rand.RandomFloat(-50.0f, 50.0f);
            _box.LinearVelocity = new Vector2(0.0f, -100.0f);
            _box.AngularVelocity = _angularVelocity;
        }

        public override void Update(GameSettings settings, GameTime gameTime)
        {
            base.Update(settings, gameTime);

            DrawString("Press C to toggle CCD. CCD is Currently: " + (_ground.IgnoreCCD ? "Off" : "On"));

            if (DistanceGJK.GJKCalls > 0)
                DrawString($"GJK calls = {DistanceGJK.GJKCalls:n}, Ave GJK iters = {DistanceGJK.GJKIters / (float)DistanceGJK.GJKCalls:n}, Max GJK iters = {DistanceGJK.GJKMaxIters:n}");

            if (TimeOfImpact.TOICalls > 0)
            {
                DrawString($"TOI calls = {TimeOfImpact.TOICalls:n}, Ave TOI iters = {TimeOfImpact.TOIIters / (float)TimeOfImpact.TOICalls:n}, Max TOI iters = {TimeOfImpact.TOIMaxRootIters:n}");
                DrawString($"Ave TOI root iters = {TimeOfImpact.TOIRootIters / (float)TimeOfImpact.TOICalls:n}, Max TOI root iters = {TimeOfImpact.TOIMaxRootIters:n}");
            }

            if (StepCount % 60 == 0)
                Launch();
        }

        public override void Keyboard(KeyboardManager keyboardManager)
        {
            base.Keyboard(keyboardManager);

            if (keyboardManager.IsNewKeyPress(Keys.C))
                _ground.IgnoreCCD = !_ground.IgnoreCCD;
        }

        internal static Test Create()
        {
            return new ContinuousTest();
        }
    }
}