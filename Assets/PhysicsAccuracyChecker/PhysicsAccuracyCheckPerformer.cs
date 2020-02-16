using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PhysicsAccuracyChecker
{
    public class PhysicsAccuracyCheckPerformer : MonoBehaviour
    {
        public PhysicsAccuracyTestSubject Prefab;
        public int SubjectsTeGenerateCount;
        public Vector3 PerSubjectDelta;

        void Start()
        {
            StartCoroutine(TestingCoroutine());
        }

        private IEnumerator TestingCoroutine()
        {
            yield return new WaitForSeconds(4);

            int? centerIndex=null;
            var subjects = Enumerable.Range(-SubjectsTeGenerateCount, SubjectsTeGenerateCount*2)
                .Select((i,indexInList) =>
                {
                    if (i == 0)
                    {
                        centerIndex = indexInList;
                    }
                    var o = GameObject.Instantiate(Prefab, transform);
                    var delta = PerSubjectDelta * i;
                    o.transform.localPosition = delta;
                    return new TestSubjectWithDelta()
                    {
                        Delta = delta,
                        Subject = o
                    };
                }).ToList();

            while (!subjects.All(c => c.Subject.TestFinished))
            {
                yield return new WaitForSeconds(1);
            }

            var baseInaccuracy = subjects[centerIndex.Value].Subject.Inaccuracy;
            var baseDeltaDistance  = subjects[centerIndex.Value].Subject.DeltaDistance;
            var sb = new StringBuilder();
            subjects.ForEach(c =>
            {
                if (c.Subject.UseForce)
                {
                    sb.AppendLine(
                        $"BartekPhysicsAccuracyTest: Delta:{c.Delta} Pos:{c.Subject.transform.position} Inccuracy:{c.Subject.DeltaDistance} InChange:{(c.Subject.DeltaDistance/baseDeltaDistance) * 100}");
                }
                else
                {
                    sb.AppendLine(
                        $"BartekPhysicsAccuracyTest: Delta:{c.Delta} Pos:{c.Subject.transform.position} Inccuracy:{c.Subject.Inaccuracy} InChange:{(c.Subject.Inaccuracy / baseInaccuracy) * 100}");
                }

                GameObject.Destroy(c.Subject.gameObject);
            });
            Debug.Log(sb.ToString());
        }
    }

    public class TestSubjectWithDelta
    {
        public PhysicsAccuracyTestSubject Subject;
        public Vector3 Delta;
    }
}
