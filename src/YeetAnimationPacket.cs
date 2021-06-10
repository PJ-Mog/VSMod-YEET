using ProtoBuf;
using System.Collections.Generic;

namespace Yeet {
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class YeetAnimationPacket {
    public string YeeterUID = "";
    // string is the animation code, true to start, false to stop
    public Dictionary<string, bool> AnimationList = new Dictionary<string, bool>();

    public YeetAnimationPacket(string yeeterUID) {
      this.YeeterUID = yeeterUID;
    }

    public YeetAnimationPacket() {}

    public YeetAnimationPacket AddAnimation(string animCode, bool shouldPlay) {
      this.AnimationList.Add(animCode, shouldPlay);
      return this;
    }
  }
}
