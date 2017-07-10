﻿using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Select : UnitTestBase
    {
        private Select() { }
        public Select(int eachCount)
        {
            this.Count = eachCount;
        }
        internal void Init()
        {
            base.Begin();
            for (int i = 0; i < base.Count; i++)
            {
                single();
                single2();
                Multiple();
                singleDynamic();
                MultipleDynamic();
            }
            base.End("Select Test");
        }

        private void Multiple()
        {
            Expression<Func<Student, School, object>> exp = (it, school) => new Student() { Name = "a", Id = it.Id, SchoolId = school.Id, TestId = it.Id + 1 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.SelectMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @"  @constant0 AS [Name] , [it].[Id] AS [Id] , [school].[Id] AS [SchoolId] , ( [it].[Id] + @Id1 ) AS [TestId] ",
                new List<SugarParameter>(){
                 new SugarParameter("@constant0","a"),
                 new SugarParameter("@Id1",1)
                },
                "Select.Multiple Error");
        }

        private  void MultipleDynamic()
        {
            Expression<Func<Student, School, object>> exp = (it, school) => new { Name = "a", Id = it.Id / 2, SchoolId = school.Id };
            ExpressionContext expContext = new ExpressionContext();
            expContext.IsSingle = false;
            expContext.Resolve(exp, ResolveExpressType.SelectMultiple);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
              selectorValue,
              pars,
              @" @constant0 AS [Name] , ( [it].[Id] / @Id1 ) AS [Id] , [school].[Id] AS [SchoolId]  ",
              new List<SugarParameter>(){
                new SugarParameter("@constant0","a"),
                new SugarParameter("@Id1", 2)},
              "Select.MultipleDynamic Error");
        }
        private  void single()
        {
            int p = 1;
            Expression<Func<Student, object>> exp = it => new Student() { Name = "a", Id = it.Id, SchoolId = p,TestId=it.Id+11 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @" @constant0 AS [Name] , [Id] AS [Id] , @constant1 AS [SchoolId] , ( [Id] + @Id2 ) AS [TestId]  ",
                new List<SugarParameter>(){
                            new SugarParameter("@constant0","a"),
                            new SugarParameter("@constant1",1),
                            new SugarParameter("@Id2",11 ) },
                "Select.single Error");
        }
        private void single2(int p=1)
        {
            Expression<Func<Student, object>> exp = it => new Student() { Name = "a", Id = it.Id, SchoolId = p, TestId = it.Id + 11 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
                selectorValue,
                pars,
                @" @constant0 AS [Name] , [Id] AS [Id] , @constant1 AS [SchoolId] , ( [Id] + @Id2 ) AS [TestId]  ",
                new List<SugarParameter>(){
                            new SugarParameter("@constant0","a"),
                            new SugarParameter("@constant1",1),
                            new SugarParameter("@Id2",11 ) },
                "Select.single Error");
        }

        private  void singleDynamic()
        {
            string a = "a";
            Expression<Func<Student, object>> exp = it => new { x = it.Id, shoolid = 1, name = a,p=it.Id*2 };
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(exp, ResolveExpressType.SelectSingle);
            var selectorValue = expContext.Result.GetString();
            var pars = expContext.Parameters;
            base.Check(
            selectorValue,
            pars,
            @" [Id] AS [x] , @constant0 AS [shoolid] , @constant1 AS [name] , ( [Id] * @Id2 ) AS [p] ",
            new List<SugarParameter>(){
                                    new SugarParameter("@constant0",1),
                                    new SugarParameter("@constant1","a"),
                                    new SugarParameter("@Id2",2)},
            "Select.singleDynamic Error");
        }
    }
}