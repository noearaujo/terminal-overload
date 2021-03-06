// Copyright information can be found in the file named COPYING
// located in the root directory of this distribution.

#ifndef _SFXFMODVOICE_H_
#define _SFXFMODVOICE_H_

#ifndef _SFXDEVICE_H_
   #include "sfx/sfxDevice.h"
#endif
#ifndef _SFXVOICE_H_
   #include "sfx/sfxVoice.h"
#endif
#ifndef _BITSET_H_
   #include "core/bitSet.h"
#endif

#include "fmod.h"

class SFXSource;
class SFXFMODBuffer;
class SFXFMODDevice;


class SFXFMODVoice : public SFXVoice
{
      typedef SFXVoice Parent;
      friend class SFXFMODBuffer;

   protected:

      SFXFMODDevice *mDevice;

      mutable FMOD_CHANNEL *mChannel;
      
      enum ESettings
      {
         SET_MinMaxDistance   = BIT( 0 ),
         SET_Velocity         = BIT( 1 ),
         SET_Transform        = BIT( 2 ),
         SET_Volume           = BIT( 3 ),
         SET_Pitch            = BIT( 4 ),
         SET_Cone             = BIT( 5 ),
         SET_Priority         = BIT( 6 ),
         SET_Reverb           = BIT( 7 ),
      };
      
      BitSet32 mSetFlags;
      
      FMOD_MODE mMode;
      F32 mMinDistance;
      F32 mMaxDistance;
      F32 mVolume;
      F32 mPriority;
      F32 mFrequency;
      F32 mConeInnerAngle;
      F32 mConeOuterAngle;
      F32 mConeOuterVolume;
      FMOD_VECTOR mVelocity;
      FMOD_VECTOR mPosition;
      FMOD_VECTOR mDirection;
      FMOD_REVERB_CHANNELPROPERTIES mReverb;

      ///
	   SFXFMODVoice(  SFXFMODDevice *device, 
                     SFXFMODBuffer *buffer );

      // prep for playback
      bool _assignChannel();

      SFXFMODBuffer* _getBuffer() const { return ( SFXFMODBuffer* ) mBuffer.getPointer(); }

      // SFXVoice.
      virtual SFXStatus _status() const;
      virtual void _play();
      virtual void _pause();
      virtual void _stop();
      virtual void _seek( U32 sample );
      virtual U32 _tell() const;

   public:

      ///
      static SFXFMODVoice* create(  SFXFMODDevice *device, 
                                    SFXFMODBuffer *buffer );

      ///
      virtual ~SFXFMODVoice();

      /// SFXVoice
      void setMinMaxDistance( F32 min, F32 max );
      void play( bool looping );
      void setVelocity( const VectorF& velocity );
      void setTransform( const MatrixF& transform );
      void setVolume( F32 volume );
      void setPriority( F32 priority );
      void setPitch( F32 pitch );
      void setCone( F32 innerAngle, F32 outerAngle, F32 outerVolume );
      void setReverb( const SFXSoundReverbProperties& reverb );
      bool isVirtual() const;
};

#endif // _SFXFMODBUFFER_H_