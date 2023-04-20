using Microsoft.EntityFrameworkCore;
using HeyListen.Models;

namespace HeyListen.Data
{
  public class HeyListenDbContext : DbContext
  {
    public HeyListenDbContext(DbContextOptions<HeyListenDbContext> options) : base(options)
    {
    }

    public DbSet<Channel> Channels { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserChannel> UserChannels { get; set; }

    public static void SeedData(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>().HasData(
          new User
          {
            sub = "cd061435-79a4-4484-be7a-88b3ea6ff827",
            username = "chicken"
          });

      modelBuilder.Entity<Channel>().HasData(
          new Channel
          {
            Id = 1,
            Name = "ElectroVibes",
            Description = "A high-energy EDM channel featuring the latest electronic dance music hits, remixes, and exclusive live sets from top DJs and producers around the world.",
            AuthorId = "cd061435-79a4-4484-be7a-88b3ea6ff827"
          },
          new Channel
          {
            Id = 2,
            Name = "Rockin' Classics",
            Description = "A classic rock channel dedicated to the iconic bands and songs from the 60s, 70s, and 80s, featuring legends like Led Zeppelin, Pink Floyd, and The Rolling Stones.",
            AuthorId = "cd061435-79a4-4484-be7a-88b3ea6ff827"
          },
          new Channel
          {
            Id = 3,
            Name = "HipHopNation",
            Description = "A channel that celebrates the rich culture of hip-hop, featuring the latest releases, classic tracks, interviews with industry insiders, and exclusive content from up-and-coming artists.",
            AuthorId = "cd061435-79a4-4484-be7a-88b3ea6ff827"
          });

      modelBuilder.Entity<Message>().HasData(
        new Message
        {
          Id = 1,
          Text = "Hey, what's up?",
          SenderId = "cd061435-79a4-4484-be7a-88b3ea6ff827",
          ChannelId = 1,
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

      // Add this line to create the sequence for Channels table
      modelBuilder.HasSequence<int>("channels_id_seq");

      // Add this line to create the sequence for Messages table
      modelBuilder.HasSequence<int>("messages_id_seq");

      modelBuilder.Entity<UserChannel>()
          .HasKey(uc => new { uc.UserId, uc.ChannelId });

      modelBuilder.Entity<UserChannel>()
          .HasOne(uc => uc.User)
          .WithMany(u => u.UserChannels)
          .HasForeignKey(uc => uc.UserId);

      modelBuilder.Entity<UserChannel>()
          .HasOne(uc => uc.Channel)
          .WithMany(c => c.UserChannels)
          .HasForeignKey(uc => uc.ChannelId);

      modelBuilder.Entity<Message>()
        .HasOne(m => m.Sender)
        .WithMany(u => u.Messages)
        .HasForeignKey(m => m.SenderId);

      SeedData(modelBuilder);

      base.OnModelCreating(modelBuilder);
    }
  }
}







//     public DbSet<Channel> Channels { get; set; }
//     public DbSet<Message> Messages { get; set; }
//     public DbSet<User> Users { get; set; }
//     public DbSet<UserChannel> UserChannels { get; set; }



//     // public async Task<Channel> CreatePrivateChannelAsync(User user1, User user2)
//     // {
//     //   var privateChannel = new Channel
//     //   {
//     //     Name = $"Private-{user1.sub}-{user2.sub}",
//     //     Description = $"Private channel between {user1.username} and {user2.username}",
//     //     IsPrivate = true,
//     //     AuthorId = user1.sub,
//     //     Author = user1
//     //   };

//     //   Channels.Add(privateChannel);

//     //   var userChannel1 = new UserChannel
//     //   {
//     //     User = user1,
//     //     Channel = privateChannel
//     //   };

//     //   var userChannel2 = new UserChannel
//     //   {
//     //     User = user2,
//     //     Channel = privateChannel
//     //   };

//     //   privateChannel.UserChannels.Add(userChannel1);
//     //   privateChannel.UserChannels.Add(userChannel2);

//     //   await SaveChangesAsync();

//     //   return privateChannel;
//     // }

//     // public async Task<Channel> AddUserToChannelAsync(Channel channel, User user)
//     // {
//     //   if (channel.IsPrivate && channel.UserChannels.Count == 2)
//     //   {
//     //     // Create a new channel for all 3 users
//     //     var newChannel = new Channel
//     //     {
//     //       Name = $"Group-{channel.Id}-{user.sub}",
//     //       Description = $"Group channel for {channel.Name} and {user.username}",
//     //       IsPrivate = true,
//     //       AuthorId = channel.AuthorId,
//     //       Author = channel.Author
//     //     };

//     //     Channels.Add(newChannel);

//     //     // Add the existing users to the new channel
//     //     foreach (var userChannel in channel.UserChannels)
//     //     {
//     //       var newUserChannel = new UserChannel
//     //       {
//     //         User = userChannel.User,
//     //         Channel = newChannel
//     //       };

//     //       newChannel.UserChannels.Add(newUserChannel);
//     //     }

//     //     // Add the new user to the new channel
//     //     var newUserChannelToAdd = new UserChannel
//     //     {
//     //       User = user,
//     //       Channel = newChannel
//     //     };

//     //     newChannel.UserChannels.Add(newUserChannelToAdd);

//     //     await SaveChangesAsync();

//     //     return newChannel;
//     //   }
//     //   else
//     //   {
//     //     // Simply add the new user to the existing channel
//     //     var userChannel = new UserChannel
//     //     {
//     //       User = user,
//     //       Channel = channel
//     //     };

//     //     channel.UserChannels.Add(userChannel);
//     //     await SaveChangesAsync();

//     //     return channel;
//     //   }
//     // }

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//       modelBuilder.Entity<Channel>(entity =>
//       {
//         entity.Property(e => e.Id).ValueGeneratedOnAdd();
//       });

//       modelBuilder.Entity<Message>(entity =>
//       {
//         entity.Property(e => e.Id).ValueGeneratedOnAdd();
//       });
//       modelBuilder.Entity<UserChannel>()
//             .HasKey(uc => new { uc.UserId, uc.ChannelId });

//       modelBuilder.Entity<UserChannel>()
//           .HasOne(uc => uc.User)
//           .WithMany(u => u.UserChannels)
//           .HasForeignKey(uc => uc.UserId);

//       modelBuilder.Entity<UserChannel>()
//           .HasOne(uc => uc.Channel)
//           .WithMany(c => c.UserChannels)
//           .HasForeignKey(uc => uc.ChannelId);

//       modelBuilder.Entity<Message>()
//           .HasOne(m => m.Sender)
//           .WithMany(u => u.Messages)
//           .HasForeignKey(m => m.SenderId);

//       SeedData(modelBuilder);
//     }
//   }
// }