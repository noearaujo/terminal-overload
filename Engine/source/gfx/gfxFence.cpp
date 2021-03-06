// Copyright information can be found in the file named COPYING
// located in the root directory of this distribution.

#include "platform/platform.h"
#include "gfx/gfxFence.h"

#include "gfx/primBuilder.h"
#include "gfx/gfxTextureManager.h"


GFXGeneralFence::~GFXGeneralFence()
{
   // Release the ref pointers
   mRenderTarget = NULL;
   mRTTexHandle = NULL;

   // Unregister texture callback
   GFXTextureManager::removeEventDelegate( this, &GFXGeneralFence::_onTextureEvent );
}

//------------------------------------------------------------------------------

void GFXGeneralFence::_init()
{
   // Register texture callback to re-create resources
   if ( !mInitialized )
      GFXTextureManager::addEventDelegate( this, &GFXGeneralFence::_onTextureEvent );

   // Set this to true for error checking
   mInitialized = true;

   // Allocate resources
   mInitialized &= mRTTexHandle.set( 2, 2, GFXFormatR8G8B8X8, &GFXDefaultRenderTargetProfile, avar("%s() - mInitialized (line %d)", __FUNCTION__, __LINE__) );
   mRenderTarget = GFX->allocRenderToTextureTarget();
   mInitialized &= ( mRenderTarget != NULL );

   GFXStateBlockDesc d;
   mRenderSB = GFX->createStateBlock(d);
}

//------------------------------------------------------------------------------

void GFXGeneralFence::issue()
{
   PROFILE_SCOPE(GFXGeneralFence_Issue);
   
   // Resource creation will be done on first call to issue()
   if( !mInitialized )
      _init();

   // If we can't init, return.
   if(!mInitialized)
      return;

   AssertFatal( mInitialized, "Error occured during GFXGeneralFence::_init, sorry I can't be more specific, break and debug." );

   RectI viewport = GFX->getViewport();

   GFX->pushActiveRenderTarget();
   mRenderTarget->attachTexture( GFXTextureTarget::Color0, mRTTexHandle );
   GFX->setActiveRenderTarget( mRenderTarget );

   // Set-up states
   GFX->setStateBlock(mRenderSB);
   GFX->pushWorldMatrix();
   GFX->setWorldMatrix( MatrixF::Identity );

   // CodeReview: We can re-do this with a static vertex buffer at some point. [5/9/2007 Pat]
   PrimBuild::begin( GFXTriangleList, 3 );
      PrimBuild::vertex2f( 0.f, 0.f );
      PrimBuild::vertex2f( 0.f, 1.f );
      PrimBuild::vertex2f( 1.f, 0.f );
   PrimBuild::end();

   GFX->popWorldMatrix();
   GFX->popActiveRenderTarget();

   GFX->setViewport(viewport);
}

//------------------------------------------------------------------------------

void GFXGeneralFence::block()
{
   PROFILE_SCOPE(GFXGeneralFence_block);
   if( !mInitialized )
      return;

   // We have to deal with the case where the lock fails (usually due to 
   // a device reset).
   if(mRTTexHandle.lock())
      mRTTexHandle.unlock();
}

void GFXGeneralFence::_onTextureEvent( GFXTexCallbackCode code )
{
   switch( code )
   {
   case GFXZombify:
      mRTTexHandle = NULL;
      break;

   case GFXResurrect:
      mRTTexHandle.set( 2, 2, GFXFormatR8G8B8X8, &GFXDefaultRenderTargetProfile, avar("%s() - GFXGeneralFence->mRTTexHandle (line %d)", __FUNCTION__, __LINE__) );
      break;
   }
}

void GFXGeneralFence::zombify()
{
   mRTTexHandle = NULL;
}

void GFXGeneralFence::resurrect()
{
   mRTTexHandle.set( 2, 2, GFXFormatR8G8B8X8, &GFXDefaultRenderTargetProfile, avar("%s() - mRTTexHandle (line %d)", __FUNCTION__, __LINE__) );
}

const String GFXGeneralFence::describeSelf() const
{
   // We've got nothing
   return String();
}
