﻿// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

// This interface was copied directly from the MSDN Specification for
// IAsyncStateMachine from the NetFx 4.5 BCL.

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Represents state machines that are generated for asynchronous methods. This type is intended for compiler use only.
    /// </summary>
    public interface IAsyncStateMachine
    {
        /// <summary>
        /// Moves the state machine to its next state.
        /// </summary>
        void MoveNext();

        /// <summary>
        /// Configures the state machine with a heap-allocated replica.
        /// </summary>
        /// <param name="stateMachine">
        /// The heap-allocated replica.
        /// </param>
        void SetStateMachine(IAsyncStateMachine stateMachine);
    }
}