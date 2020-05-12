using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model;
using SomeProject.Core.Model.Account;
using SomeProject.Core.Repository.Account.impl;
using SomeProject.Infrastructure.Common.Exceptions;
using SomeProject.Infrastructure.Data.UnitOfWork;

namespace SomeProject.Console
{
    public class EfOneToOneTest
    {
        #region 公共调用方法

        public static void RunTest()
        {
            while (true)
            {
                try
                {
                    System.Console.WriteLine("请输入命令：0; 退出程序，功能命令：1 - n");
                    string input = System.Console.ReadLine();
                    if (input == null)
                    {
                        continue;
                    }

                    if (input == "0")
                    {
                        break;
                    }

                    Type type = MethodBase.GetCurrentMethod().DeclaringType;
                    object o = Activator.CreateInstance(type);
                    type.InvokeMember("Method" + input, BindingFlags.Default | BindingFlags.InvokeMethod, null, o,
                        new object[] { });
                }
                catch (Exception e)
                {
                    ExceptionHandler(e);
                }
            }
        }

        private static void ExceptionHandler(Exception e)
        {
            ExceptionMessage emsg = new ExceptionMessage(e);
            System.Console.WriteLine(emsg.ErrorDetails);
        }

        #endregion

        #region 新增测试,以Context的形式进行

        //可以通过Set<T>.Add实现新增
        public void Method1()
        {
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user1",
                    Password = "user1",
                    UserExpand = new SysUserExpand() { ExpandValue1 = "user1的补充" },
                };

                context.Set<SysUser>().Add(user);
                context.SaveChanges();
            }
        }

        //可以通过修改EntityState.Added实现新增
        public void Method2()
        {
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user2",
                    Password = "user2",
                    UserExpand = new SysUserExpand() { ExpandValue1 = "user2的补充" },
                };
                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        //1、2都是一次定义包含两个对象，也可以两步定义
        public void Method3()
        {
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user3",
                    Password = "user3",
                };
                var userExpand = new SysUserExpand()
                {
                    ExpandValue1 = "user3的补充"
                };
                user.UserExpand = userExpand;

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        #region 既然可以两步定义，那么若这两步是存在前后关系呢？比如User存在，为其添加附加信息

        //这里当然肯定是先存在User，再存在UserExpand
        //这里省略掉在同一个上下文操作的情况（先查出数据库的User，再添加它的UserExpand），是可以的，道理如同3；无论设置User修改或UserExpand都是可以的
        //主要测试在不同上下文的操作

        //通过新增User对象的导航属性UserExpand，然后设置User修改
        //不报错，但是达不到预期结果，UserExpand没有新增
        //注意：这里新增UserExpand是包含id和导航User的
        public void Method4()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user4",
                    Password = "user4",
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().SingleOrDefault(o => o.UserName == "user4");
            }

            SysUserExpand newUserExpand = new SysUserExpand
            {
                Id = newUser.Id,
                User = newUser,
                ExpandValue1 = "user4的补充"
            };

            newUser.UserExpand = newUserExpand;

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        //通过新增UserExpand对象，然后设置UserExpand新增
        //不报错，但是达不到预期效果，同时新增了新的User和新的UserExpand
        //注意：这里新增UserExpand是包含id和导航User的
        public void Method5()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user5",
                    Password = "user5",
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().SingleOrDefault(o => o.UserName == "user5");
            }

            SysUserExpand newUserExpand = new SysUserExpand
            {
                Id = newUser.Id,
                User = newUser,
                ExpandValue1 = "user5的补充"
            };

            newUser.UserExpand = newUserExpand;

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUserExpand).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        //在例子4、5中，UserExpand被添加了id和User，那么只有id或只有User？

        //按4测试设置User修改，但UserExpand只有ID的情况
        //结果还是同4，不报错，但是达不到预期结果，UserExpand没有新增
        public void Method6()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user6",
                    Password = "user6",
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().SingleOrDefault(o => o.UserName == "user6");
            }

            SysUserExpand newUserExpand = new SysUserExpand
            {
                Id = newUser.Id,
                ExpandValue1 = "user6的补充"
            };

            newUser.UserExpand = newUserExpand;

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        //按4再测试只有User的情况
        //直接报错，关系中的外键值不能匹配了
        public void Method7()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user7",
                    Password = "user7",
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().SingleOrDefault(o => o.UserName == "user7");
            }

            SysUserExpand newUserExpand = new SysUserExpand
            {
                User = newUser,
                ExpandValue1 = "user6的补充"
            };

            newUser.UserExpand = newUserExpand;

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        //按5测试设置UserExpand新增，但UserExpand只有ID的情况
        //正确
        public void Method8()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user8",
                    Password = "user8",
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().SingleOrDefault(o => o.UserName == "user8");
            }

            SysUserExpand newUserExpand = new SysUserExpand
            {
                Id = newUser.Id,
                ExpandValue1 = "user8的补充"
            };

            newUser.UserExpand = newUserExpand;

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUserExpand).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        //按5再测试只有User的情况
        //结果还是同5，不报错，但是达不到预期效果，同时新增了新的User和新的UserExpand
        public void Method9()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user9",
                    Password = "user9",
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().SingleOrDefault(o => o.UserName == "user9");
            }

            SysUserExpand newUserExpand = new SysUserExpand
            {
                User = newUser,
                ExpandValue1 = "user9的补充"
            };

            newUser.UserExpand = newUserExpand;

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUserExpand).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }
        }

        #endregion

        #endregion

        #region 修改测试,以Context的形式进行

        //主表、从表单独修改就不测试了，肯定可以的，这里测试主表、从表一起修改的情况

        //假设在同一个上下文中，已有User，对User和UserExpand进行修改，但只标注User修改如何
        //达到目标
        public void Method10()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user10",
                    Password = "user10",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user10的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            using (var context = new DefaultDbContext())
            {
                var user = context.Set<SysUser>().SingleOrDefault(o => o.UserName == "user10");

                user.UserName = "修改的user10";
                user.UserExpand.ExpandValue1 = "修改的user10的补充";

                context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        //假设对User和UserExpand进行修改是不在上下文的，但只标注User修改如何
        //未达到目标，user被修改但userexpand未修改
        public void Method11()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user11",
                    Password = "user11",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user11的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().Include(p => p.UserExpand)
                    .SingleOrDefault(o => o.UserName == "user11");
            }

            newUser.UserName = "修改的user11";
            newUser.UserExpand.ExpandValue1 = "修改的user11的补充";

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        //假设在同一个上下文中，已有UserExpand，对User和UserExpand进行修改，但只标注userExpand修改如何
        //达到目标
        public void Method12()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user12",
                    Password = "user12",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user12的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            using (var context = new DefaultDbContext())
            {
                var userExpand = context.Set<SysUserExpand>().SingleOrDefault(o => o.ExpandValue1 == "user12的补充");

                userExpand.User.UserName = "修改的user12";
                userExpand.ExpandValue1 = "修改的user12的补充";

                context.Entry(userExpand).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        //假设对User和UserExpand进行修改是不在上下文的，但只标注UserExpand修改如何
        //未达到目标，userexpand被修改但user未修改
        public void Method13()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user13",
                    Password = "user13",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user13的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUserExpand newUserExpand = null;
            using (var context = new DefaultDbContext())
            {
                newUserExpand = context.Set<SysUserExpand>().Include(p => p.User)
                    .SingleOrDefault(o => o.ExpandValue1 == "user13的补充");
            }

            newUserExpand.User.UserName = "修改的user13";
            newUserExpand.ExpandValue1 = "修改的user13的补充";

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUserExpand).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        //很明显了，对不在上下文的修改，必须标注user和userexpand都被修改
        public void Method14()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user14",
                    Password = "user14",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user14的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().Include(p => p.UserExpand)
                    .SingleOrDefault(o => o.UserName == "user14");
            }

            newUser.UserName = "修改的user14";
            newUser.UserExpand.ExpandValue1 = "修改的user14的补充";

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
                context.Entry(newUser.UserExpand).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }


        #endregion

        #region 删除测试,以Context的形式进行

        //只标注User删除，按提示是可以，但因为这里的ID是int，删除的效果是从表主键为null，故无法做到才报错 
        public void Method15()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user15",
                    Password = "user15",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user15的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().Include(p => p.UserExpand)
                    .SingleOrDefault(o => o.UserName == "user15");
            }

            //{"The operation failed: The relationship could not be changed because one or more of the foreign-key properties is non-nullable. When a change is made to a relationship, the related foreign-key property is set to a null value. If the foreign-key does not support null values, a new relationship must be defined, the foreign-key property must be assigned another non-null value, or the unrelated object must be deleted."}

            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                context.Entry(newUser).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        //只标注UserExpand删除，很容易理解，当然成功了
        public void Method16()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user16",
                    Password = "user16",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user16的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUserExpand newUserExpand = null;
            using (var context = new DefaultDbContext())
            {
                newUserExpand = context.Set<SysUserExpand>().Include(p => p.User)
                    .SingleOrDefault(o => o.ExpandValue1 == "user16的补充");
            }

            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                context.Entry(newUserExpand).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        //同时标注User删除和UserExpand删除，通过User，先标注User删除再标注User.UserExpand删除
        //报错，原因在context.Entry(newUser).State = System.Data.Entity.EntityState.Deleted;之后newUser.UserExpand==null
        public void Method17()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user17",
                    Password = "user17",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user17的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().Include(p => p.UserExpand)
                    .SingleOrDefault(o => o.UserName == "user17");
            }

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUser).State = System.Data.Entity.EntityState.Deleted;
                context.Entry(newUser.UserExpand).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        //会不会是外键的影响？那反过来，通过UserExpand，先标注newUserExpand删除再标注newUserExpand.User删除
        //还是报错，原因还是如上
        public void Method18()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user18",
                    Password = "user18",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user18的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUserExpand newUserExpand = null;
            using (var context = new DefaultDbContext())
            {
                newUserExpand = context.Set<SysUserExpand>().Include(p => p.User)
                    .SingleOrDefault(o => o.ExpandValue1 == "user18的补充");
            }

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUserExpand).State = System.Data.Entity.EntityState.Deleted;
                context.Entry(newUserExpand.User).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        //回头再看17，那我先标注User.UserExpand删除再标注User删除
        //到达预期
        public void Method19()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user19",
                    Password = "user19",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user19的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUser newUser = null;
            using (var context = new DefaultDbContext())
            {
                newUser = context.Set<SysUser>().Include(p => p.UserExpand)
                    .SingleOrDefault(o => o.UserName == "user19");
            }

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUser.UserExpand).State = System.Data.Entity.EntityState.Deleted;
                context.Entry(newUser).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        //想想18，也按这种思路改一改先标注newUserExpand.User删除再标注newUserExpand删除
        //也可以达到预期
        public void Method20()
        {
            //准备一条数据
            using (var context = new DefaultDbContext())
            {
                var user = new SysUser()
                {
                    UserName = "user20",
                    Password = "user20",
                    UserExpand = new SysUserExpand
                    {
                        ExpandValue1 = "user20的补充"
                    }
                };

                context.Entry(user).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
            }

            SysUserExpand newUserExpand = null;
            using (var context = new DefaultDbContext())
            {
                newUserExpand = context.Set<SysUserExpand>().Include(p => p.User)
                    .SingleOrDefault(o => o.ExpandValue1 == "user20的补充");
            }

            using (var context = new DefaultDbContext())
            {
                context.Entry(newUserExpand.User).State = System.Data.Entity.EntityState.Deleted;
                context.Entry(newUserExpand).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        #endregion

        /*  总结
         *  一步新增：总是在一个上下文里的，不提
         *  两步新增：当新增是上下文内的，如查询或Attach，是可以的（自动追踪未关闭的前提下，一般在这时不会关闭除非查询，都是这个场景下文不再说了）
         *            当新增是上下文外的，只要包含导航属性，就不能达到预期。故只能是从表新增，且不包含导航属性
         *
         *
         *  两表同时修改：修改是上下文内的，只标注其中一个是修改的是可以的，两个修改更可以了         
         *                修改是上下文外的，必须标注两个都修改才可以
         *
         *  两表同时删除：只标注主表删除可以，但这种删除的效果是从表主键为null，主键一般不为null，且这种删除遗留的数据还需要另外处理 
         *                同时标注两个删除更加可以，但特别需要注意一旦对象被标注删除，它的导航属性也就不存在了，2个对象稳妥，1个对象要注意顺序
         */

        //一步新增，通过User仓储
        public void Method100()
        {
            var repo = new UserRepository();
            var user = new SysUser()
            {
                UserName = "user100",
                Password = "user100",
                UserExpand = new SysUserExpand() { ExpandValue1 = "user100的补充" },
            };
            repo.Insert(user);
        }

        //一步新增，通过UserExpand仓储
        public void Method101()
        {
            var repo = new UserExpandRepository();
            var userExpand = new SysUserExpand()
            {
                ExpandValue1 = "user101的补充",
                User = new SysUser()
                {
                    UserName = "user101",
                    Password = "user101"
                }
            };
            repo.Insert(userExpand);

        }

        //两步新增，比较可能的错误
        public void Method102()
        {
            var repo = new UserRepository();
            var user = new SysUser()
            {
                UserName = "user102",
                Password = "user102",
            };
            repo.Insert(user);

            var newUser = repo.Entities.SingleOrDefault(o => o.UserName == "user102");
            newUser.UserExpand = new SysUserExpand()
            {
                Id = newUser.Id,
                User = newUser,
                ExpandValue1 = "user102的补充"
            };
            repo.Update(newUser);


            //    SysUserExpand newUserExpand = new SysUserExpand
            //    {
            //        Id = newUser.Id,
            //        ExpandValue1 = "user102的补充"
            //    };

            //    using (var repo = new UserExpandRepository())
            //    {
            //        repo.Insert(newUserExpand);
            //    }
        }

        //两步新增，从上面Context的测试可以看到只能从表新增
        public void Method103()
        {
            var repo = new UserRepository();
            var user = new SysUser()
            {
                UserName = "user103",
                Password = "user103",
            };
            repo.Insert(user);

            var newUser = repo.Entities.SingleOrDefault(o => o.UserName == "user103");
            newUser.UserExpand = new SysUserExpand()
            {
                Id = newUser.Id,
                ExpandValue1 = "user103的补充"
            };
            repo.Update(newUser);

            var repo1 = new UserExpandRepository();
            repo1.Insert(newUser.UserExpand);
        }

        //两表同时修改
        public void Method104()
        {
            var repo = new UserRepository();
            var user = new SysUser()
            {
                UserName = "user104",
                Password = "user104",
                UserExpand = new SysUserExpand
                {
                    ExpandValue1 = "user104的补充"
                }
            };
            repo.Insert(user);

            SysUser newUser = repo.Entities.Include(p => p.UserExpand).SingleOrDefault(o => o.UserName == "user104");
            newUser.UserName = "修改的user104";
            newUser.UserExpand.ExpandValue1 = "修改的user104的补充";

            repo.Update(newUser, false);

            var repo1 = new UserExpandRepository();
            repo1.Update(newUser.UserExpand, false);
            repo.Save();
        }

        //删除测试，这样还是如同Context一样，主键不能为空
        public void Method105()
        {
            var repo = new UserRepository();
            var user = new SysUser()
            {
                UserName = "user105",
                Password = "user105",
                UserExpand = new SysUserExpand
                {
                    ExpandValue1 = "user105的补充"
                }
            };
            repo.Insert(user);

            long id = repo.Entities.Include(p => p.UserExpand).SingleOrDefault(o => o.UserName == "user105").Id;
            var newUser = repo.Query(id);
            repo.Delete(newUser);
        }

        public void Method106()
        {
            var repo = new UserRepository();
            var user = new SysUser()
            {
                UserName = "user106",
                Password = "user106",
                UserExpand = new SysUserExpand
                {
                    ExpandValue1 = "user106的补充"
                }
            };
            repo.Insert(user);

            long id = repo.Entities.Include(p => p.UserExpand).SingleOrDefault(o => o.UserName == "user106").Id;
            var newUser = repo.Query(id);
            repo.Delete(newUser, false);

            var repo1 = new UserExpandRepository();
            var newUserExpand = repo1.Query(id);
            repo1.Delete(newUserExpand, false);

            repo.Save();
        }
    }
}
