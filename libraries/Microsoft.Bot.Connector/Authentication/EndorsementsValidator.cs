﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Bot.Connector.Authentication
{
    public static class EndorsementsValidator
    {
        /// <summary>
        /// Verify that the set of ChannelIds, which come from the incoming activities,
        /// all match the endorsements found on the JWT Token. 
        /// For example, if an Activity comes from DirectLine, that channelId should
        /// say that, and the jwt token endorsement must also match that. 
        /// </summary>
        /// <param name="channelId">The channel name, typically extracted from the activity.ChannelId field, that
        /// to which the Activity is affinitized.</param>
        /// <param name="endorsements">Whoever signed the JWT token is permitted to send activities only for
        /// some specific channels. That list is the endorsement list, and is validated here against the channelId.</param>
        /// <param name="allowUnendorsedChannels">In channel development scenarios, a channel will send activities
        /// prior to being able to sign a JWT token. When debugging that channel, it's necessary to allow
        /// unendorsed activities to be acted on. Setting this to true effectivly disables the endorsement
        /// validation.</param>
        /// <returns>True is the channelId is found in the Endorsement set. False if the channelId is not found.</returns>
        public static bool Validate(string channelId, string[] endorsements, bool allowUnendorsedChannels = false)
        {
            // If the Activity came in and doesn't have a Channel ID then it's making no 
            // assertions as to who endorses it. This means it should pass. 
            if (string.IsNullOrEmpty(channelId))
                return true;

            // The scenario of allowing this is when a new channel is being developed and 
            // the endorsements are not yet working. When this happens, developers will put
            // this in a "Debug" mode, and allow unendorsed channels.  
            if (allowUnendorsedChannels)            
                return true;            

            if (endorsements == null)
                throw new ArgumentNullException(nameof(endorsements));

            // Because this is less than clear, and seems to take a while to figure out, the
            // call path to get here is: 
            // JwtTokenValidation.AuthenticateRequest
            //  ->
            //   JwtTokenValidation.ValidateAuthHeader
            //    ->                                         
            //      ChannelValidation.AuthenticateChannelToken
            //       -> 
            //          JWTTokenExtractor

            // Does the set of endorsements match the channelId that was passed in? 
            bool endorsementPresent = endorsements.Contains(channelId);
            return endorsementPresent;
        }
    }
}
